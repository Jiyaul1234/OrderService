namespace Ecommerce.OrderService.Domain.Enums
{
    /// <summary>
    /// Represents a derived summary status for API responses, combining payment and shipment states.
    /// </summary>
    public enum OrderSummaryStatusEnum
    {
        /// <summary>Order is awaiting payment confirmation.</summary>
        PendingPayment = 1,

        /// <summary>Order is being processed for shipment.</summary>
        Processing = 2,

        /// <summary>Order fulfillment is complete.</summary>
        Completed = 3
    }
}
