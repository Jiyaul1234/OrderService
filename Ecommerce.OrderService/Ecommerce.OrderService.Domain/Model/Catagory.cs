using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ecommerce.OrderService.Domain.Model
{
    [Table("Catagory")]
    public class Catagory
    {
        [Key]
        public int CatagoryId { get; set; }
        public string CatagoryName
        {
            get; set;
        }
    }
}
