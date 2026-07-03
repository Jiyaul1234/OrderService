using System;
using System.Collections.Generic;

namespace Ecommerce.OrderService.Application.DTOs
{
    public class ShippingAddressDto
    {
        public string FullName { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class OrderDto
    {
        public string Id { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string OrderNumber { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        public decimal Subtotal { get; set; }
        public decimal ShippingCharges { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public ShippingAddressDto ShippingAddress { get; set; } = new ShippingAddressDto();
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = "pending";
        public string OrderStatus { get; set; } = "pending";
        public string ShipmentStatus { get; set; } = "pending";
        public string? TrackingNumber { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
