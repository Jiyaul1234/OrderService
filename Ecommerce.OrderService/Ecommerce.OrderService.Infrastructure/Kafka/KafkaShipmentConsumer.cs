using Confluent.Kafka;
using Ecommerce.OrderService.Application.DTOs;
using Ecommerce.OrderService.Application.Events;
using Ecommerce.OrderService.Application.Interface.IReposiotory;
using Ecommerce.OrderService.Application.Interface.IService;
using Ecommerce.OrderService.Domain.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Text.Json;

namespace Ecommerce.OrderService.Infrastructure.Kafka
{
    public class KafkaShipmentConsumer : IHostedService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<KafkaShipmentConsumer> _logger;
        private IConsumer<string, string>? _consumer;
       
        private CancellationTokenSource? _cts;
        private Task? _consumerTask;

        public KafkaShipmentConsumer(IConfiguration configuration, ILogger<KafkaShipmentConsumer> logger, IServiceScopeFactory scopeFactory)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
            _logger = logger;

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Kafka consumer...");

            var bootstrapServers = _configuration["Kafka:BootstrapServers"];
            var topic = _configuration["Kafka:PaymentconfirmdTopic"];
            var groupId = _configuration["Kafka:GroupId"] ?? "payment-service-group";

            if (string.IsNullOrWhiteSpace(bootstrapServers) || string.IsNullOrWhiteSpace(topic))
            {
                _logger.LogError("Kafka configuration missing. Ensure Kafka:BootstrapServers and Kafka:Topic are set.");
                return Task.CompletedTask;
            }

            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            _consumer = new ConsumerBuilder<string, string>(config)
                .SetErrorHandler((_, e) => _logger.LogError("Kafka error: {Reason}", e.Reason))
                .Build();

            _consumer.Subscribe(topic);

            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Run the consume loop on a background task
            _consumerTask = Task.Run(async () =>
            {
                var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                while (!_cts.Token.IsCancellationRequested)
                {
                    try
                {
                    var result = _consumer.Consume(_cts.Token);
                    if (result?.Message == null) continue;

                    _logger.LogInformation("Consumed message from Kafka. Topic: {Topic}, Partition: {Partition}, Offset: {Offset}, Key: {Key}",
                        result.Topic, result.Partition, result.Offset, result.Message.Key);

                    ShipmentConfirmdEvent? shipment;
                    try
                    {
                        shipment = JsonSerializer.Deserialize<ShipmentConfirmdEvent>(result.Message.Value, jsonOptions);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to deserialize Kafka message. Skipping offset {Offset}", result.Offset);
                        try { _consumer.Commit(result); } catch { }
                        continue;
                    }

                    if (shipment == null)
                    {
                        _logger.LogWarning("Deserialized shipment is null. Skipping offset {Offset}", result.Offset);
                        try { _consumer.Commit(result); } catch { }
                        continue;
                    }

                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                        var order = await orderRepository.GetByIdAsync(shipment.OrderId);
                        if (order == null)
                        {
                            _logger.LogWarning("Order {OrderId} not found while processing shipment", shipment.OrderId);
                            try { _consumer.Commit(result); } catch { }
                            continue;
                        }

                        order.UpdatedOn = DateTime.Now;
                        order.ShipmentStatus = shipment.ShipmentStatus;
                        order.TrackingNumber = shipment.TrackingNumber;
                        order.EstimatedDeliveryDate = shipment.EstimatedDeliveryDate;

                        await orderRepository.Update(order);

                        // commit only after successful processing
                        _consumer.Commit(result);
                        _logger.LogInformation("Processed and committed message offset {Offset}", result.Offset);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing shipment for OrderId {OrderId}. Leaving offset uncommitted for retry.", shipment.OrderId);
                        // Do not commit; message will be re-delivered according to Kafka consumer group semantics
                    }
                }
                catch (OperationCanceledException)
                {
                    // shutdown requested
                    break;
                }
                catch (ConsumeException cex)
                {
                    _logger.LogError(cex, "Consume exception");
                    await Task.Delay(TimeSpan.FromSeconds(1), _cts.Token).ContinueWith(_ => { });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in Kafka consume loop");
                    await Task.Delay(TimeSpan.FromSeconds(1), _cts.Token).ContinueWith(_ => { });
                }
                }
            }, _cts.Token);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Kafka consumer...");

            try
            {
                _cts?.Cancel();

                if (_consumerTask != null)
                {
                    await Task.WhenAny(_consumerTask, Task.Delay(TimeSpan.FromSeconds(10), cancellationToken));
                }

                _consumer?.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping Kafka consumer");
            }
        }

        public void Dispose()
        {
            _consumer?.Dispose();
            _cts?.Dispose();
        }


        private async Task publishToshipMent(Order order)
        {
            string topicName = _configuration["Kafka:PaymentconfirmdTopic"]?.ToString() ?? string.Empty;

            var shippingAddress = string.IsNullOrEmpty(order.ShippingAddressJson)
                ? new ShippingAddressDto()
                : JsonSerializer.Deserialize<ShippingAddressDto>(order.ShippingAddressJson)!;

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

            using var scoped = _scopeFactory.CreateScope();
            var producer = scoped.ServiceProvider.GetRequiredService<IKafkaProducer>();
            await producer.PublishAsync<ShipmentCreatedEvent>(topicName, shipmentCreatedEvent);
        }
    }
}
