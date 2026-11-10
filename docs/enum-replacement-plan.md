**Comprehensive Enum Replacement Plan**

## Business Domain Enums to Create

### 1. OrderStatusEnum (Order workflow status)
- "pending" → OrderStatus.Pending
- Future states: Processing, Shipped, Delivered, Cancelled, Returned

### 2. PaymentStatusEnum (Payment lifecycle status)
- "pending" → PaymentStatus.Pending
- "Succeeded" → PaymentStatus.Succeeded
- Future states: Failed, Refunded, Disputed

### 3. ShipmentStatusEnum (Shipment workflow status)
- "pending" → ShipmentStatus.Pending
- Future states: Picked, Packed, Shipped, InTransit, Delivered, Returned

### 4. OrderSummaryStatusEnum (Derived order status for API response)
- "PendingPayment" → OrderSummaryStatus.PendingPayment
- "Processing" → OrderSummaryStatus.Processing
- "Completed" → OrderSummaryStatus.Completed

## Replacement Mapping

### Order Entity (Domain/Model/Order.cs)
- Line 33: OrderStatus = "pending" → OrderStatus = OrderStatus.Pending.ToString()
- Line 34: ShipmentStatus = "pending" → ShipmentStatus = ShipmentStatus.Pending.ToString()
- Property types: string → keep string for JSON serialization (store enum name)

### OrdersController (API/Controllers/OrdersController.cs)
- Line 71: string.Equals(dto.PaymentStatus, "pending", ...) → dto.PaymentStatus == PaymentStatus.Pending.ToString()
- Line 73: string.Equals(dto.ShipmentStatus, "pending", ...) → dto.ShipmentStatus == ShipmentStatus.Pending.ToString()
- Line 74: status = "PendingPayment" → status = OrderSummaryStatus.PendingPayment.ToString()
- Line 75: status = "Processing" → status = OrderSummaryStatus.Processing.ToString()
- Line 77: status = "Completed" → status = OrderSummaryStatus.Completed.ToString()

### ServiceBusPaymentProcessor (Infrastructure/BusService/ServiceBusPaymentProcessor.cs)
- Line 83: if (payment.Status == "Succeeded") → if (payment.Status == PaymentStatus.Succeeded.ToString())

### Note: Configuration Keys, CORS, JWT, and Technical Constants
- NOT converted to enums (these are configuration-driven, not business logic)
- MaxConcurrentCalls (5) is a performance tuning constant, not a business enum
- ConnectionString, FQNS, TopicNames are environment-specific, not business logic

## Serialization Strategy
- Enums serialized as strings (enum.ToString()) to preserve database compatibility
- JSON serialization handled by default JsonSerializer (PropertyNameCaseInsensitive: true)
- No migration needed for existing string data in database
