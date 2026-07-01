using Ecommerce.OrderService.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.OrderService.Infrastructure
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Order { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Catagory> Catagory { get; set; }

    }
}
