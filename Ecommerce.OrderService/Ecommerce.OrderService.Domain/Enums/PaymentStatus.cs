using System.ComponentModel;

namespace Ecommerce.OrderService.Domain.Enums
{
    /// <summary>
    /// Represents the payment status for an order.
    /// </summary>
    public enum PaymentStatusEnum
    {
        /// <summary>Payment is awaiting processing.</summary>
        [Description("Pending")] 
        Pending = 1,

        /// <summary>Payment has been successfully processed.</summary>
        [Description("Succeeded")] 
        Succeeded = 2,

        /// <summary>Payment processing failed.</summary>
        [Description("Failed")]
        Failed = 3,

        /// <summary>Payment has been refunded.</summary>
        [Description("Refunded")]
        Refunded = 4,

        /// <summary>Payment is under dispute.</summary>
        [Description("Disputed")]
        Disputed = 5
    }
}
