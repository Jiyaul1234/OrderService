using Ecommerce.OrderService.Application.Interface.IReposiotory;
using Ecommerce.OrderService.Domain.Model;
using Microsoft.Extensions.Logging;

namespace Ecommerce.OrderService.Infrastructure.Reposiotory
{
    public class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(AppDbContext dbContext, ILogger<OrderItemRepository> logger) : base(dbContext, logger)
        {
        }
    }
}
