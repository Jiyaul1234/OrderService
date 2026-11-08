using Azure.Messaging.ServiceBus;
using Ecommerce.OrderService.Application.Events;
using Ecommerce.OrderService.Application.Interface.IReposiotory;
using Ecommerce.OrderService.Application.Interface.IService;
using Ecommerce.OrderService.Application.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Ecommerce.OrderService.Infrastructure.BusService
{
    public class ServiceBusPaymentProcessor : IHostedService, IAsyncDisposable
    {
        private readonly ServiceBusClient _client;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ServiceBusPaymentProcessor> _logger;
        private ServiceBusProcessor? _processor;

        public ServiceBusPaymentProcessor(ServiceBusClient client, IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<ServiceBusPaymentProcessor> logger)
        {
            _client = client;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var topic = _configuration["AzureServiceBus:PaymentTopic"];
            var subscription = _configuration["AzureServiceBus:SubscriptionForPayment"];
            if (string.IsNullOrWhiteSpace(topic) || string.IsNullOrWhiteSpace(subscription))
            {
                _logger.LogWarning("Service Bus payment topic or subscription not configured");
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
            PaymentCreatedEvent? payment = null;
            try
            {
                payment = JsonSerializer.Deserialize<PaymentCreatedEvent>(body, jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize payment message");
            }

            if (payment == null)
            {
                await args.CompleteMessageAsync(args.Message);
                return;
            }

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                var producer = scope.ServiceProvider.GetRequiredService<IserviceBusProducer>();
                var order = await orderRepository.GetByIdAsync(payment.OrderId);
                if (order != null)
                {
                    order.UpdatedOn = DateTime.Now;
                    order.PaymentStatus = payment.Status;
                    await orderRepository.Update(order);
                    if (payment.Status == "Succeeded")
                    {
                        // publish shipment event
                        var cfg = _configuration["AzureServiceBus:PaymentConfirmedTopic"] ?? _configuration["AzureServiceBus:PaymentconfirmdTopic"];
                        var shippingAddress = string.IsNullOrEmpty(order.ShippingAddressJson)
                            ? new ShippingAddressDto()
                            : JsonSerializer.Deserialize<ShippingAddressDto>(order.ShippingAddressJson!);

                        var shipmentCreatedEvent = new ShipmentCreatedEvent
                        {
                            OrderId = order.OrderId,
                            CreatedOn = DateTime.Now,
                            FullName = shippingAddress?.FullName ?? string.Empty,
                            AddressLine1 = shippingAddress?.AddressLine1 ?? string.Empty,
                            AddressLine2 = shippingAddress?.AddressLine2 ?? string.Empty,
                            City = shippingAddress?.City ?? string.Empty,
                            Country = shippingAddress?.Country ?? string.Empty,
                            PostalCode = shippingAddress?.PostalCode ?? string.Empty,
                            PhoneNumber = shippingAddress?.PhoneNumber ?? string.Empty,
                            MessageId = Guid.NewGuid().ToString(),
                            State = shippingAddress?.State ?? string.Empty
                        };

                        if (!string.IsNullOrWhiteSpace(cfg))
                        {
                            await producer.PublishAsync(cfg, shipmentCreatedEvent);
                        }
                    }
                }

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment message");
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
