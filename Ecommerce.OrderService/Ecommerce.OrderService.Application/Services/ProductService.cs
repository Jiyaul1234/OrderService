using AutoMapper;
using Ecommerce.OrderService.Application.Interface.IReposiotory;
using Ecommerce.OrderService.Application.Interface.IService;
using Ecommerce.OrderService.Application.DTOs;
using Ecommerce.OrderService.Domain.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.OrderService.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepository;
        private readonly IMapper mapper;
        private readonly ILogger<ProductService> logger;

        public ProductService(IProductRepository productRepository, IMapper mapper, ILogger<ProductService> logger)
        {
            this.productRepository = productRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task AddAsync(ProductDto productDto)
        {
            if (productDto == null)
            {
                logger.LogWarning("Received null product DTO for creation");
                throw new ArgumentNullException(nameof(productDto));
            }

            if (string.IsNullOrWhiteSpace(productDto.Name) || string.IsNullOrWhiteSpace(productDto.Category))
            {
                logger.LogWarning("Product creation with missing fields: Name={Name}, Category={Category}", 
                    string.IsNullOrWhiteSpace(productDto.Name) ? "missing" : "provided",
                    string.IsNullOrWhiteSpace(productDto.Category) ? "missing" : "provided");
            }

            logger.LogInformation("Adding product {ProductName}", productDto.Name);

            var product = mapper.Map<Product>(productDto);

            await productRepository.AddAsync(product);

            logger.LogInformation("Added product {ProductId} with price {Price}", product.Id, product.Price);
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            logger.LogInformation("Getting all products");

            var products = await productRepository.GetAllAsync();
            var result = products.Select(p => mapper.Map<ProductDto>(p)).ToList();

            logger.LogInformation("Retrieved {Count} products", result.Count);
            return result;
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            logger.LogInformation("Getting product by id {ProductId}", id);

            var p = await productRepository.GetByIdAsync(id);
            if (p == null)
            {
                logger.LogWarning("Product {ProductId} not found", id);
                return null;
            }

            var dto = mapper.Map<ProductDto>(p);

            logger.LogInformation("Found product {ProductId} with stock {Stock}", id, p.Stock);
            return dto;
        }

        public async Task RemoveAsync(int id)
        {
            logger.LogInformation("Removing product {ProductId}", id);

            var p = await productRepository.GetByIdAsync(id);
            if (p == null)
            {
                logger.LogWarning("Product {ProductId} not found for removal", id);
                return;
            }

            if (p.Stock > 0)
            {
                logger.LogWarning("Removing product {ProductId} with stock remaining: {Stock}", id, p.Stock);
            }

            await productRepository.Remove(p);

            logger.LogInformation("Removed product {ProductId}", id);
        }

        public async Task UpdateAsync(ProductDto productDto)
        {
            logger.LogInformation("Updating product {ProductId}", productDto.Id);

            var p = await productRepository.GetByIdAsync(productDto.Id);
            if (p == null)
            {
                logger.LogWarning("Product {ProductId} not found for update", productDto.Id);
                return;
            }

            var oldPrice = p.Price;
            var oldStock = p.Stock;
            var oldDiscount = p.Discount;

            mapper.Map(productDto, p);

            if (oldPrice != p.Price)
            {
                logger.LogInformation("Product {ProductId} price changed: {OldPrice} -> {NewPrice}", productDto.Id, oldPrice, p.Price);
            }
            if (oldStock != p.Stock)
            {
                logger.LogInformation("Product {ProductId} stock changed: {OldStock} -> {NewStock}", productDto.Id, oldStock, p.Stock);
            }
            if (oldDiscount != p.Discount)
            {
                logger.LogInformation("Product {ProductId} discount changed: {OldDiscount} -> {NewDiscount}", productDto.Id, oldDiscount, p.Discount);
            }

            await productRepository.Update(p);

            logger.LogInformation("Updated product {ProductId}", productDto.Id);
        }
    }
}
