using Ecommerce.OrderService.Application.Interface.IReposiotory;
using Ecommerce.OrderService.Domain.Model;
using Microsoft.Extensions.Logging;

namespace Ecommerce.OrderService.Infrastructure.Reposiotory
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        private AppDbContext _context;
        public OrderRepository(AppDbContext dbContext, ILogger<OrderRepository> logger) : base(dbContext, logger)
        {
            _context = dbContext;
        }

        public async  Task<int> AddOrder(Order entity)
        {
           await  _context.Set<Order>().AddAsync(entity);
           await _context.SaveChangesAsync();
           return entity.OrderId;
        }
        
    }
}
