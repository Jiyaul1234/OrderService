using AutoMapper;
using Ecommerce.OrderService.Application.DTOs;
using Ecommerce.OrderService.Domain.Model;
using System.Globalization;
using System.Text.Json;
using Ecommerce.OrderService.Application.Events;

namespace Ecommerce.OrderService.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Order mappings
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OrderId.ToString(CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.UserId.ToString(CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderNumber))
                .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Subtotal))
                .ForMember(dest => dest.ShippingCharges, opt => opt.MapFrom(src => src.ShippingCharges))
                .ForMember(dest => dest.Tax, opt => opt.MapFrom(src => src.Tax))
                .ForMember(dest => dest.GrandTotal, opt => opt.MapFrom(src => src.GrandTotal))
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.ShippingAddressJson) ? new ShippingAddressDto() : JsonSerializer.Deserialize<ShippingAddressDto>(src.ShippingAddressJson)!))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus ?? "pending"))
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus))
                .ForMember(dest => dest.ShipmentStatus, opt => opt.MapFrom(src => src.ShipmentStatus))
                .ForMember(dest => dest.TrackingNumber, opt => opt.MapFrom(src => src.TrackingNumber))
                .ForMember(dest => dest.EstimatedDeliveryDate, opt => opt.MapFrom(src => src.EstimatedDeliveryDate))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedOn))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedOn));

            CreateMap<OrderDto, Order>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => ParseInt(src.Id)))
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderNumber))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => ParseInt(src.CustomerId)))
                .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Subtotal))
                .ForMember(dest => dest.ShippingCharges, opt => opt.MapFrom(src => src.ShippingCharges))
                .ForMember(dest => dest.Tax, opt => opt.MapFrom(src => src.Tax))
                .ForMember(dest => dest.GrandTotal, opt => opt.MapFrom(src => src.GrandTotal))
                .ForMember(dest => dest.ShippingAddressJson, opt => opt.MapFrom(src => JsonSerializer.Serialize(src.ShippingAddress)))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus))
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus))
                .ForMember(dest => dest.ShipmentStatus, opt => opt.MapFrom(src => src.ShipmentStatus))
                .ForMember(dest => dest.TrackingNumber, opt => opt.MapFrom(src => src.TrackingNumber))
                .ForMember(dest => dest.EstimatedDeliveryDate, opt => opt.MapFrom(src => src.EstimatedDeliveryDate))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => src.UpdatedAt));

            // OrderItem mappings
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId.ToString(CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Price * src.Quantity));

            CreateMap<OrderItemDto, OrderItem>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => ParseInt(src.ProductId)))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));

            // Product mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString(CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock))
                .ForMember(dest => dest.InStock, opt => opt.MapFrom(src => src.InStock))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.DiscountedPrice, opt => opt.MapFrom(src => src.DiscountedPrice));

            CreateMap<ProductDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => ParseInt(src.Id)))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Stock))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.DiscountedPrice, opt => opt.MapFrom(src => src.DiscountedPrice))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));

            // Map shipment event into order: embed shipping address as JSON and update shipment status/time
            CreateMap<ShipmentCreatedEvent, Order>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.ShippingAddressJson, opt => opt.MapFrom(src => JsonSerializer.Serialize(new ShippingAddressDto
                {
                    FullName = src.FullName,
                    AddressLine1 = src.AddressLine1,
                    AddressLine2 = src.AddressLine2,
                    City = src.City,
                    State = src.State,
                    PostalCode = src.PostalCode,
                    Country = src.Country,
                    PhoneNumber = src.PhoneNumber
                })))
                .ForMember(dest => dest.ShipmentStatus, opt => opt.MapFrom(src => "shipped"))
                .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => src.CreatedOn));
        }

        private static int ParseInt(string? value)
        {
            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v)) return v;
            return 0;
        }
    }
}
