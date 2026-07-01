using Azure.Messaging.ServiceBus;
using Ecommerce.OrderService.Application.Events;
using Ecommerce.OrderService.Application.Interface.IService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

public class OrderProducer:IOrderProducerService
{
    private readonly ServiceBusSender _sender;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrderProducer> _logger;

    public OrderProducer(ServiceBusClient client, IConfiguration configuration,ILogger<OrderProducer> logger)
    {
        _configuration = configuration;
        _logger= logger;
        _sender = client.CreateSender(
            _configuration["AzureServiceBus:QueueName"]);
    }

    public async Task PublishAsync(OrderCreatedEvent order)
    {

        _logger.LogInformation($"Start publish orderCreatedEvent:{JsonSerializer.Serialize(order)}");
        var json = JsonSerializer.Serialize(order);

        var message = new ServiceBusMessage(json)
        {
            MessageId = order.MessageId,
            ContentType = "application/json"
        };

        message.ApplicationProperties.Add("EventType", "OrderCreated");

        await _sender.SendMessageAsync(message);

       _logger.LogInformation($"Published successfully orderId:{order.OrderId}");
    }
}