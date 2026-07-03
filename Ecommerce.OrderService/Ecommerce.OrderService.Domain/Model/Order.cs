using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ecommerce.OrderService.Domain.Model
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        // keep integer PK for DB but expose string Id in DTO
        public string OrderNumber { get; set; } = string.Empty;

        // original user id kept as int but mapped to CustomerId string in DTO
        public int UserId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal Subtotal { get; set; }
        public decimal ShippingCharges { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }

        // shipping address stored as JSON in DB for simplicity
        public string ShippingAddressJson { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;
        public string? PaymentStatus { get; set; }

        public string OrderStatus { get; set; } = "pending";
        public string ShipmentStatus { get; set; } = "pending";
        public string? TrackingNumber { get; set; }

        public DateTime EstimatedDeliveryDate { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
