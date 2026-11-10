using Ecommerce.OrderService.Domain.Model;

namespace Ecommerce.OrderService.Application.Interface.IReposiotory
{
    public interface IOrderItemRepository : IBaseReposiotory<OrderItem>
    {
        Task AddRange(List<OrderItem> orderItems);
    }
}
