using Ecommerce.OrderService.Application.Events;

namespace Ecommerce.OrderService.Application.Interface.IService
{
    public interface IOrderProducerService
    {
        public  Task PublishAsync(OrderCreatedEvent orderCreatedEvent); 
    }
}
