namespace Ecommerce.OrderService.Application.DTOs
{
    public class OrderItemDto
    {
        public int ItemId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
