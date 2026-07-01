using Ecommerce.OrderService.Application.Interface.IReposiotory;
using Ecommerce.OrderService.Domain.Model;
using Microsoft.Extensions.Logging;

namespace Ecommerce.OrderService.Infrastructure.Reposiotory
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext dbContext, ILogger<ProductRepository> logger) : base(dbContext, logger)
        {
        }
    }
}
