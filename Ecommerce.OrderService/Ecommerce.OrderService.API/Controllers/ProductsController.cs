using Ecommerce.OrderService.Application.Interface.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Ecommerce.OrderService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService;
        private readonly ILogger<ProductsController> logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            this.productService = productService;
            this.logger = logger;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            logger.LogInformation("Received request to get all products");

            var dtos = await productService.GetAllAsync();

            logger.LogInformation("Returning products (count may vary)");
            return Ok(dtos);
        }

        // GET: api/products/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            logger.LogInformation("Received request to get product {ProductId}", id);

            var dto = await productService.GetByIdAsync(id);
            if (dto == null)
            {
                logger.LogInformation("Product {ProductId} not found", id);
                return NotFound();
            }

            logger.LogInformation("Returning product {ProductId}", id);
            return Ok(dto);
        }
    }
}
