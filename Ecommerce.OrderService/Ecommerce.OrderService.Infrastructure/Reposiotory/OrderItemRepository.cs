using Ecommerce.OrderService.Application.Interface.IReposiotory;
using Ecommerce.OrderService.Domain.Model;
using Microsoft.Extensions.Logging;

namespace Ecommerce.OrderService.Infrastructure.Reposiotory
{
    public class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
    {
        private readonly AppDbContext _dbContext;
        public OrderItemRepository(AppDbContext dbContext, ILogger<OrderItemRepository> logger) : base(dbContext, logger)
        {
            _dbContext = dbContext;
            
        }

        public async Task AddRange(List<OrderItem> orderItems) 
        {
          await  _dbContext.AddRangeAsync(orderItems);
          await _dbContext.SaveChangesAsync();
        }
    }
}

