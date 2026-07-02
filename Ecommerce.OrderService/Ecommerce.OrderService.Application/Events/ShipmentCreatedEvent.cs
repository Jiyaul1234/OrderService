using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.OrderService.Application.Events
{
    public  class ShipmentCreatedEvent
    {
        public string MessageId { get; set; }
        public int OrderId { get; set; }   
        public DateTime CreatedOn { get; set; }
    }
}
