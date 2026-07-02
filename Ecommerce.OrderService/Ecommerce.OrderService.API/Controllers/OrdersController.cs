using Ecommerce.OrderService.Application.DTOs;
using Ecommerce.OrderService.Application.Interface.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Ecommerce.OrderService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService orderService;
        private readonly ILogger<OrdersController> logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            this.orderService = orderService;
            this.logger = logger;
        }

        // POST: api/orders
        [HttpPost]
      
        public async Task<IActionResult> Create([FromBody] OrderDto orderDto)
        {
            if (orderDto == null)
                return BadRequest();

            logger.LogInformation("Received request to create order for user {UserId}", orderDto.UserId);

            await orderService.AddAsync(orderDto);

            logger.LogInformation("Order create request processed for user {UserId}", orderDto.UserId);

            // return created with location to GetById
            return CreatedAtAction(nameof(GetById), new { id = orderDto.OrderId }, orderDto);
        }

        // GET: api/orders/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetById(int id)
        {
            logger.LogInformation("Received request to get order {OrderId}", id);

            var dto = await orderService.GetByIdAsync(id);
            if (dto == null)
            {
                logger.LogInformation("Order {OrderId} not found", id);
                return NotFound();
            }

            logger.LogInformation("Returning order {OrderId}", id);
            return Ok(dto);
        }

        // GET: api/orders/{id}/status
        [HttpGet("{id:int}/status")]
        public async Task<IActionResult> GetStatus(int id)
        {
            logger.LogInformation("Received request for status of order {OrderId}", id);

            var dto = await orderService.GetByIdAsync(id);
            if (dto == null) return NotFound();

            string status;
            // simple status derivation
            if (dto.PaymentId == 0)
                status = "PendingPayment";
            else if (dto.ShippingId == 0)
                status = "Processing";
            else
                status = "Completed";

            logger.LogInformation("Order {OrderId} status: {Status}", id, status);
            return Ok(new { OrderId = id, Status = status });
        }
    }
}
