using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ecommerce.OrderService.Domain.Model
{
    [Table("Product")]
    public  class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string Name { get; set; }
        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
