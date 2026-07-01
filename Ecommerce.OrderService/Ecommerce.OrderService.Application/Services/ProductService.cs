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
            logger.LogInformation("Adding product {ProductName}", productDto.Name);

            var product = mapper.Map<Product>(productDto);

            await productRepository.AddAsync(product);

            logger.LogInformation("Added product {ProductId}", product.ProductId);
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
                logger.LogInformation("Product {ProductId} not found", id);
                return null;
            }

            var dto = mapper.Map<ProductDto>(p);

            logger.LogInformation("Found product {ProductId}", id);
            return dto;
        }

        public async Task RemoveAsync(int id)
        {
            logger.LogInformation("Removing product {ProductId}", id);

            var p = await productRepository.GetByIdAsync(id);
            if (p == null)
            {
                logger.LogInformation("Product {ProductId} not found for removal", id);
                return;
            }

            await productRepository.Remove(p);

            logger.LogInformation("Removed product {ProductId}", id);
        }

        public async Task UpdateAsync(ProductDto productDto)
        {
            logger.LogInformation("Updating product {ProductId}", productDto.ProductId);

            var p = await productRepository.GetByIdAsync(productDto.ProductId);
            if (p == null)
            {
                logger.LogInformation("Product {ProductId} not found for update", productDto.ProductId);
                return;
            }

            mapper.Map(productDto, p);

            await productRepository.Update(p);

            logger.LogInformation("Updated product {ProductId}", productDto.ProductId);
        }
    }
}
