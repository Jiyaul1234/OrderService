using Ecommerce.OrderService.Application.Interface.IReposiotory;
using Ecommerce.OrderService.Domain.Model;
using Microsoft.Extensions.Logging;

namespace Ecommerce.OrderService.Infrastructure.Reposiotory
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext dbContext, ILogger<OrderRepository> logger) : base(dbContext, logger)
        {
        }
    }
}
