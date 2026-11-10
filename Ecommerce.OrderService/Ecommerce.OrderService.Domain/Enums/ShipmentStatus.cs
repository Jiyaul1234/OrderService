namespace Ecommerce.OrderService.Domain.Enums
{
    /// <summary>
    /// Represents the shipment status for an order.
    /// </summary>
    public enum ShipmentStatusEnum
    {
        /// <summary>Shipment has not yet been initiated.</summary>
        Pending = 1,

        /// <summary>Order items are being picked from inventory.</summary>
        Picked = 2,

        /// <summary>Order items have been packed for shipment.</summary>
        Packed = 3,

        /// <summary>Order has been shipped.</summary>
        Shipped = 4,

        /// <summary>Order is in transit to the customer.</summary>
        InTransit = 5,

        /// <summary>Order has been delivered.</summary>
        Delivered = 6,

        /// <summary>Order is being returned.</summary>
        Returned = 7
    }
}
