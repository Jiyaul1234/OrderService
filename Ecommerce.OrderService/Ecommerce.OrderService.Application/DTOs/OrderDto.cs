using System;
using System.Collections.Generic;

namespace Ecommerce.OrderService.Application.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public int? PaymentId { get; set; }
        public int? ShippingId { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime CreatedOn { get; set; }
       public DateTime UpdatedOn { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}
