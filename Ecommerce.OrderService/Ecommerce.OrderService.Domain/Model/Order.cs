using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ecommerce.OrderService.Domain.Model
{
    [Table("Order")]
    public  class Order
    {
        [Key]
        public int OrderId { get; set; }

        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }

        public int PaymentId { get; set; }

        public int ShippingId { get; set; }
    }
}
