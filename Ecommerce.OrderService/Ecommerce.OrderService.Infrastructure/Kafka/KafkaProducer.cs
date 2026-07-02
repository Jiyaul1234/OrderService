using Confluent.Kafka;
using Ecommerce.OrderService.Application.Interface.IService;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Ecommerce.OrderService.Infrastructure.Kafka
{
    public class KafkaProducer:IKafkaProducer
    {
        private readonly IProducer<string, string> _producer;
        public KafkaProducer(IConfiguration configuration)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                Acks = Acks.All,
                EnableIdempotence = true
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task PublishAsync<T>(string topic, T message)
        {
            var json = JsonSerializer.Serialize(message);

            await _producer.ProduceAsync(topic,
                new Message<string, string>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = json
                });
        }
    }
}
