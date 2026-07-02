using AutoMapper;
using Ecommerce.OrderService.Application.DTOs;
using Ecommerce.OrderService.Application.Events;
using Ecommerce.OrderService.Application.Interface.IReposiotory;
using Ecommerce.OrderService.Application.Interface.IService;
using Ecommerce.OrderService.Domain.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ecommerce.OrderService.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository orderRepository;
        private readonly IOrderItemRepository orderItemRepository;
        private readonly IMapper mapper;
        private readonly ILogger<OrderService> logger;
        private readonly IKafkaProducer orderProducerService;
        private readonly IConfiguration _configuration;
        private readonly string topicName;
        public OrderService(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IMapper mapper, 
            ILogger<OrderService> logger, IKafkaProducer orderProducerService,IConfiguration configuration)
        {
            this.orderRepository = orderRepository;
            this.orderItemRepository = orderItemRepository;
            this.mapper = mapper;
            this.logger = logger;
            this.orderProducerService = orderProducerService;
           
        }

        public async Task AddAsync(OrderDto orderDto)
        {
            decimal amount = 0;
            logger.LogInformation("Creating order for user {UserId}", orderDto.UserId);

            var order = mapper.Map<Order>(orderDto);

            int orderId = await orderRepository.AddOrder(order);

            // map and add items with generated OrderId
            if (orderDto.Items != null && orderDto.Items.Any())
            {
                foreach (var itemDto in orderDto.Items)
                {
                    var item = mapper.Map<OrderItem>(itemDto);
                    item.OrderId = order.OrderId;
                    await orderItemRepository.AddAsync(item);

                    amount += itemDto.Price;
                }
            }

            await PublishOrderMessage(orderId, amount);

            logger.LogInformation("Created order {OrderId} for user {UserId}", order.OrderId, order.UserId);
        }

        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            logger.LogInformation("Getting all orders");

            var orders = await orderRepository.GetAllAsync();
            var result = new List<OrderDto>();

            foreach (var o in orders)
            {
                var items = await orderItemRepository.FindAsync(i => i.OrderId == o.OrderId);
                var dto = mapper.Map<OrderDto>(o);
                dto.Items = items.Select(i => mapper.Map<OrderItemDto>(i)).ToList();
                result.Add(dto);
            }

            logger.LogInformation("Retrieved {Count} orders", result.Count);
            return result;
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            logger.LogInformation("Getting order by id {OrderId}", id);

            var o = await orderRepository.GetByIdAsync(id);
            if (o == null)
            {
                logger.LogInformation("Order {OrderId} not found", id);
                return null;
            }

            var items = await orderItemRepository.FindAsync(i => i.OrderId == o.OrderId);
            var dto = mapper.Map<OrderDto>(o);
            dto.Items = items.Select(i => mapper.Map<OrderItemDto>(i)).ToList();

            logger.LogInformation("Found order {OrderId} with {Count} items", id, dto.Items?.Count ?? 0);
            return dto;
        }

        public async Task RemoveAsync(int id)
        {
            logger.LogInformation("Removing order {OrderId}", id);

            var order = await orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                logger.LogInformation("Order {OrderId} not found for removal", id);
                return;
            }

            var items = await orderItemRepository.FindAsync(i => i.OrderId == order.OrderId);
            foreach (var item in items)
            {
                await orderItemRepository.Remove(item);
            }

            await orderRepository.Remove(order);

            logger.LogInformation("Removed order {OrderId}", id);
        }

        public async Task UpdateAsync(OrderDto orderDto)
        {
            decimal amount = 0;
            logger.LogInformation("Updating order {OrderId}", orderDto.OrderId);

            var order = await orderRepository.GetByIdAsync(orderDto.OrderId);
            if (order == null)
            {
                logger.LogInformation("Order {OrderId} not found for update", orderDto.OrderId);
                return;
            }

            mapper.Map(orderDto, order);

            await orderRepository.Update(order);

            // naive item sync: delete existing and add incoming
            var existingItems = await orderItemRepository.FindAsync(i => i.OrderId == order.OrderId);
            foreach (var it in existingItems)
            {
                await orderItemRepository.Remove(it);
            }

            if (orderDto.Items != null && orderDto.Items.Any())
            {
                foreach (var itemDto in orderDto.Items)
                {
                    var item = mapper.Map<OrderItem>(itemDto);
                    item.OrderId = order.OrderId;
                    await orderItemRepository.AddAsync(item);
                    amount += itemDto.Price;
                }
            }

            await PublishForshipment(order.OrderId);
            logger.LogInformation("Updated order {OrderId}", orderDto.OrderId);
        }


        private async Task PublishOrderMessage(int OrderId, decimal amount)
        {
            string topicName = _configuration["Kafka:OrderTopic"].ToString();
            OrderCreatedEvent orderCreatedEvent = new OrderCreatedEvent();
            orderCreatedEvent.OrderId = OrderId;
            orderCreatedEvent.MessageId =Guid.NewGuid().ToString();
            orderCreatedEvent.Amount = amount;
            orderCreatedEvent.CreatedOn = DateTime.Now;
            await orderProducerService.PublishAsync<OrderCreatedEvent>(topicName,orderCreatedEvent);
        }

        private async Task PublishForshipment(int OrderId)
        {
            string topicName = _configuration["Kafka:PaymenConfirmd"].ToString();
            ShipmentCreatedEvent shipmentCreatedEvent = new ShipmentCreatedEvent()
            {
                MessageId= Guid.NewGuid().ToString(),
                OrderId= OrderId,
                CreatedOn= DateTime.Now,

            };
            await orderProducerService.PublishAsync<ShipmentCreatedEvent>(topicName, shipmentCreatedEvent);
        }
    }
}
