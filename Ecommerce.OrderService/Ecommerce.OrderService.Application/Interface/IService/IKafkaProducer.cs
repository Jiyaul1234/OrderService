namespace Ecommerce.OrderService.Application.Interface.IService
{
    public interface IKafkaProducer
    {
        Task PublishAsync<T>(string topic, T message);
    }
}
