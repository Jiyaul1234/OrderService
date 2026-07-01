using AutoMapper;
using Ecommerce.OrderService.Application.DTOs;
using Ecommerce.OrderService.Domain.Model;

namespace Ecommerce.OrderService.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
        }
    }
}
