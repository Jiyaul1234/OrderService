using Ecommerce.OrderService.Application.DTOs;
using Ecommerce.OrderService.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.OrderService.Application.Interface.IService
{
    public interface IOrderService
    {
        Task<OrderDto?> GetByIdAsync(int id);
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task AddAsync(OrderDto order);
        Task UpdateAsync(OrderDto order);
        Task RemoveAsync(int id);
    }
}
