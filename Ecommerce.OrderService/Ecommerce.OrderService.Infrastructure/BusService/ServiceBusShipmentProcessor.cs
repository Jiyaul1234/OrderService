using Azure.Messaging.ServiceBus;
using Ecommerce.OrderService.Application.Events;
using Ecommerce.OrderService.Application.Interface.IReposiotory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Ecommerce.OrderService.Infrastructure.BusService
{
    public class ServiceBusShipmentProcessor : IHostedService, IAsyncDisposable
    {
        private readonly ServiceBusClient _client;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ServiceBusShipmentProcessor> _logger;
        private ServiceBusProcessor? _processor;

        public ServiceBusShipmentProcessor(ServiceBusClient client, IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<ServiceBusShipmentProcessor> logger)
        {
            _client = client;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var topic = _configuration["AzureServiceBus:PaymentConfirmedTopic"];
            var subscription = _configuration["AzureServiceBus:SubscriptionForShipment"];
            if (string.IsNullOrWhiteSpace(topic) || string.IsNullOrWhiteSpace(subscription))
            {
                _logger.LogWarning("Service Bus shipment topic or subscription not configured");
                return Task.CompletedTask;
            }

            _processor = _client.CreateProcessor(topic, subscription, new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 5,
                AutoCompleteMessages = false
            });

            _processor.ProcessMessageAsync += ProcessMessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            return _processor.StartProcessingAsync(cancellationToken);
        }

        private async Task ProcessMessageHandler(ProcessMessageEventArgs args)
        {
            var body = args.Message.Body.ToString();
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            ShipmentConfirmdEvent? shipment = null;
            try
            {
                shipment = JsonSerializer.Deserialize<ShipmentConfirmdEvent>(body, jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize shipment message");
            }

            if (shipment == null)
            {
                await args.CompleteMessageAsync(args.Message);
                return;
            }

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                var order = await orderRepository.GetByIdAsync(shipment.OrderId);
                if (order == null)
                {
                    _logger.LogWarning("Order {OrderId} not found while processing shipment", shipment.OrderId);
                    await args.CompleteMessageAsync(args.Message);
                    return;
                }

                order.UpdatedOn = DateTime.Now;
                order.ShipmentStatus = shipment.ShipmentStatus;
                order.TrackingNumber = shipment.TrackingNumber;
                order.EstimatedDeliveryDate = shipment.EstimatedDeliveryDate;

                await orderRepository.Update(order);

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing shipment message");
                await args.AbandonMessageAsync(args.Message);
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Service Bus error");
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_processor != null)
            {
                await _processor.StopProcessingAsync(cancellationToken);
                await _processor.DisposeAsync();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_processor != null)
            {
                await _processor.DisposeAsync();
            }
        }
    }
}
