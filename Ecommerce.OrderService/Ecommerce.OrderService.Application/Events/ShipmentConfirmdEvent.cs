using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.OrderService.Application.Events
{
    public  class ShipmentConfirmdEvent
    {
        public string MessageId { get; set; }
        public string ShipmentStatus { get; set; }
        public string TrackingNumber { get; set; }

        public int OrderId { get; set; }

        public DateTime EstimatedDeliveryDate { get; set; }

        public DateTime CreateOn { get; set; }
    }
}
