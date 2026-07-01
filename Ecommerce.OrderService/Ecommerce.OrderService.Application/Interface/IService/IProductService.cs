using Ecommerce.OrderService.Application.DTOs;
using Ecommerce.OrderService.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.OrderService.Application.Interface.IService
{
    public interface IProductService
    {
        Task<ProductDto?> GetByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task AddAsync(ProductDto product);
        Task UpdateAsync(ProductDto product);
        Task RemoveAsync(int id);
    }
}
