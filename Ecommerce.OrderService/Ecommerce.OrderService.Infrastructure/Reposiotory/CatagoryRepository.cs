using Ecommerce.OrderService.Application.Interface.IReposiotory;
using Ecommerce.OrderService.Domain.Model;
using Microsoft.Extensions.Logging;

namespace Ecommerce.OrderService.Infrastructure.Reposiotory
{
    public class CatagoryRepository : BaseRepository<Catagory>, ICatagoryRepository
    {
        public CatagoryRepository(AppDbContext dbContext, ILogger<CatagoryRepository> logger) : base(dbContext, logger)
        {
        }
    }
}
