using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ecommerce.OrderService.Domain.Model
{
    public class OrderItem
    {
        [Key]
        public int ItemId { get; set; }

        [ForeignKey("OrderId")]
        public int OrderId { get; set; }

        // store product id as string in DTO, domain keeps int but allow string mapping elsewhere
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        [NotMapped]
        public decimal TotalPrice => Price * Quantity;
    }
}
