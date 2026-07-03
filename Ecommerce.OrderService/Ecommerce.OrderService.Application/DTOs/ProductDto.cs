namespace Ecommerce.OrderService.Application.DTOs
{
    public class ProductDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Image { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int Reviews { get; set; }
        public int Stock { get; set; }
        public bool InStock { get; set; }
        public decimal? Discount { get; set; }
        public decimal? DiscountedPrice { get; set; }
    }
}