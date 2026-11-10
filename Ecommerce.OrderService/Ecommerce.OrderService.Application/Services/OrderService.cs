using AutoMapper;
using Ecommerce.OrderService.Application.DTOs;
using Ecommerce.OrderService.Application.Events;
using Ecommerce.OrderService.Application.Interface.IReposiotory;
using Ecommerce.OrderService.Application.Interface.IService;
using Ecommerce.OrderService.Domain.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Ecommerce.OrderService.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository orderRepository;
        private readonly IOrderItemRepository orderItemRepository;
        private readonly IMapper mapper;
        private readonly ILogger<OrderService> logger;
        private readonly IserviceBusProducer orderProducerService;
        private readonly IConfiguration _configuration;
        private readonly string topicName;
        public OrderService(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IMapper mapper, 
            ILogger<OrderService> logger, IserviceBusProducer orderProducerService,IConfiguration configuration)
        {
            this.orderRepository = orderRepository;
            this.orderItemRepository = orderItemRepository;
            this.mapper = mapper;
            this.logger = logger;
            this.orderProducerService = orderProducerService;
            _configuration = configuration;
        }

        public async Task AddAsync(OrderDto orderDto)
        {

            try
            {

            if (orderDto is null)
            {
                logger.LogWarning("Received null order DTO for creation");
                throw new ArgumentNullException(nameof(orderDto));
            }

            logger.LogInformation("Creating order for customer {CustomerId}", orderDto.CustomerId);

            var order = mapper.Map<Order>(orderDto);

            int orderId = await orderRepository.AddOrder(order);

            // set generated id back to dto
            orderDto.Id = orderId.ToString(System.Globalization.CultureInfo.InvariantCulture);

            // map and add items with generated OrderId
            if (orderDto.Items != null && orderDto.Items.Any())
            {
                var items = mapper.Map<List<OrderItem>>(orderDto.Items);
                    items.ForEach(x => x.OrderId = orderId);
                   await orderItemRepository.AddRange(items);

                  logger.LogInformation("Order {OrderId} created with {ItemCount} items for customer {CustomerId}", orderId, orderDto.Items.Count, orderDto.CustomerId);
            }
            else
            {
                logger.LogWarning("Order {OrderId} created with no items for customer {CustomerId}", orderId, orderDto.CustomerId);
            }        
                //await PublishOrderMessage(orderId, amount);
                logger.LogInformation("Created order {OrderId} for customer {CustomerId}", orderId, order.UserId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,$"Failed to place order orderDetails: {orderDto}");
                throw;
            }
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
                logger.LogWarning("Order {OrderId} not found", id);
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
                logger.LogWarning("Order {OrderId} not found for removal", id);
                return;
            }

            var items = await orderItemRepository.FindAsync(i => i.OrderId == order.OrderId);
            foreach (var item in items)
            {
                await orderItemRepository.Remove(item);
            }

            await orderRepository.Remove(order);

            logger.LogInformation("Removed order {OrderId} and {ItemCount} related items", id, items.Count());
        }

        //public async Task UpdateAsync(OrderDto orderDto)
        //{
        //    decimal amount = 0;
        //    logger.LogInformation("Updating order {OrderId}", orderDto.Id);

        //    var orderIdInt = int.TryParse(orderDto.Id, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var parsedId) ? parsedId : 0;
        //    var order = await orderRepository.GetByIdAsync(orderIdInt);
        //    if (order == null)
        //    {
        //        logger.LogWarning("Order {OrderId} not found for update", orderDto.Id);
        //        return;
        //    }

        //    var oldStatus = order.OrderStatus;
        //    var oldPaymentStatus = order.PaymentStatus;
        //    var oldShipmentStatus = order.ShipmentStatus;

        //    mapper.Map(orderDto, order);

        //    await orderRepository.Update(order);

        //    // Log state transitions
        //    if (oldStatus != order.OrderStatus)
        //    {
        //        logger.LogInformation("Order {OrderId} status transitioned: {OldStatus} -> {NewStatus}", orderIdInt, oldStatus, order.OrderStatus);
        //    }
        //    if (oldPaymentStatus != order.PaymentStatus)
        //    {
        //        logger.LogInformation("Order {OrderId} payment status changed: {OldPaymentStatus} -> {NewPaymentStatus}", orderIdInt, oldPaymentStatus, order.PaymentStatus);
        //    }
        //    if (oldShipmentStatus != order.ShipmentStatus)
        //    {
        //        logger.LogInformation("Order {OrderId} shipment status changed: {OldShipmentStatus} -> {NewShipmentStatus}", orderIdInt, oldShipmentStatus, order.ShipmentStatus);
        //    }

        //    // naive item sync: delete existing and add incoming
        //    var existingItems = await orderItemRepository.FindAsync(i => i.OrderId == order.OrderId);
        //    foreach (var it in existingItems)
        //    {
        //        await orderItemRepository.Remove(it);
        //    }

        //    if (orderDto.Items != null && orderDto.Items.Any())
        //    {
        //        foreach (var itemDto in orderDto.Items)
        //        {
        //            var item = mapper.Map<OrderItem>(itemDto);
        //            item.OrderId = order.OrderId;
        //            await orderItemRepository.AddAsync(item);
        //            amount += itemDto.Price * itemDto.Quantity;
        //        }
        //    }

        //    try
        //    {
        //        await PublishForshipment(order.OrderId);
        //        logger.LogInformation("Order {OrderId} sent for shipment", order.OrderId);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, "Failed to publish shipment event for order {OrderId}", order.OrderId);
        //        throw;
        //    }

        //    logger.LogInformation("Updated order {OrderId}", order.OrderId);
        //}


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

    }
}
