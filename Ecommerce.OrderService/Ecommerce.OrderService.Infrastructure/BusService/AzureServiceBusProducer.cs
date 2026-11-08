using Azure.Messaging.ServiceBus;
using Ecommerce.OrderService.Application.Interface.IService;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Ecommerce.OrderService.Infrastructure.BusService
{
    public class AzureServiceBusProducer : IserviceBusProducer
    {
        private readonly ServiceBusClient _client;
        private readonly ILogger<AzureServiceBusProducer> _logger;

        public AzureServiceBusProducer(ServiceBusClient client, ILogger<AzureServiceBusProducer> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task PublishAsync<T>(string qeueName, T message)
        {
            var json = JsonSerializer.Serialize(message);
            var sbMessage = new ServiceBusMessage(json)
            {
                ContentType = "application/json",
                MessageId = Guid.NewGuid().ToString()
            };

            var sender = _client.CreateSender(qeueName);
            try
            {
                await sender.SendMessageAsync(sbMessage);
                _logger.LogInformation("Sent message to Service Bus qeue {qeueName}", qeueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send message to Service Bus qeue {qeueName}", qeueName);
                throw;
            }
        }
    }
}
