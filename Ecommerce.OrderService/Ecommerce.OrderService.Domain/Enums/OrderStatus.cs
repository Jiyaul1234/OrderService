namespace Ecommerce.OrderService.Domain.Enums
{
    /// <summary>
    /// Represents the status of an order throughout its lifecycle.
    /// </summary>
    public enum OrderStatusEnum
    {
        /// <summary>Order awaiting payment confirmation.</summary>
        Pending = 1,

        /// <summary>Order is being prepared for shipment.</summary>
        Processing = 2,

        /// <summary>Order has been shipped.</summary>
        Shipped = 3,

        /// <summary>Order has been delivered to the customer.</summary>
        Delivered = 4,

        /// <summary>Order has been cancelled.</summary>
        Cancelled = 5,

        /// <summary>Order is being returned.</summary>
        Returned = 6
    }
}
