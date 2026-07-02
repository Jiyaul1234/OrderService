using Confluent.Kafka;
using Ecommerce.OrderService.Application.DTOs;
using Ecommerce.OrderService.Application.Events;
using Ecommerce.OrderService.Application.Interface.IService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Ecommerce.OrderService.Infrastructure.Kafka
{
    public class KafkaConsumer : IHostedService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<KafkaConsumer> _logger;
        private IConsumer<string, string>? _consumer;
        private CancellationTokenSource? _cts;
        private Task? _consumerTask;

        public KafkaConsumer(IConfiguration configuration, ILogger<KafkaConsumer> logger, IServiceScopeFactory scopeFactory)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
            _logger = logger;

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Kafka consumer...");

            var bootstrapServers = _configuration["Kafka:BootstrapServers"];
            var topic = _configuration["Kafka:PaymentTopic"];
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

                        PaymentCreatedEvent? payment;
                        try
                        {
                            payment = JsonSerializer.Deserialize<PaymentCreatedEvent>(result.Message.Value, jsonOptions);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to deserialize Kafka message. Skipping offset {Offset}", result.Offset);
                            // commit offset to avoid repeated failures (policy decision)
                            try { _consumer.Commit(result); } catch { /* ignore commit errors */ }
                            continue;
                        }

                        if (payment == null)
                        {
                            _logger.LogWarning("Deserialized order is null. Skipping offset {Offset}", result.Offset);
                            try { _consumer.Commit(result); } catch { }
                            continue;
                        }

                        var orderDto = new OrderDto
                        {
                            PaymentId = payment.PaymentId,
                            UpdatedOn = DateTime.Now,
                            PaymentStatus = payment.Status

                        };
                        try
                        {
                            using var scope = _scopeFactory.CreateScope();
                            var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                            await orderService.UpdateAsync(orderDto);
                            // commit only after successful processing
                            _consumer.Commit(result);
                            _logger.LogInformation("Processed and committed message offset {Offset}", result.Offset);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing payment for OrderId {OrderId}. Leaving offset uncommitted for retry.", orderDto.OrderId);
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
    }
}
