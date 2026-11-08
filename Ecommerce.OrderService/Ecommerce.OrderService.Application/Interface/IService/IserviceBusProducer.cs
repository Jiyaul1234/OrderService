namespace Ecommerce.OrderService.Application.Interface.IService
{
    public interface IserviceBusProducer
    {
        Task PublishAsync<T>(string qeueName, T message);
    }
}
