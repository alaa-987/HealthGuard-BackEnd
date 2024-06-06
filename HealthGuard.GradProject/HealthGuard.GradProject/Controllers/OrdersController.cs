using AutoMapper;
using HealthGuard.Core.Services.contract;
using HealthGuard.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HealthGuard.GradProject.DTO;
using HealthGuard.GradProject.Errors;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HealthGuard.Core.Entities.Order;
using HealthGuard.Core.Repository.contract;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using HealthGuard.Core.Specifications.OrderSpecification;

namespace HealthGuard.GradProject.Controllers
{
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepository _basketRepo;

        public OrdersController(IOrderService orderService, IMapper mapper, IUnitOfWork unitOfWork,IBasketRepository basketRepo)
        {
            _orderService = orderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _basketRepo = basketRepo;
        }
        [HttpPost("{basketId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(string basketId, [FromBody] OrderDto orderDto)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { Message = "User email not found in token" });
            }

            var basketIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "BasketId");
            basketId = basketIdClaim?.Value;

            if (string.IsNullOrEmpty(basketId))
            {
                return BadRequest(new { Message = "Basket ID not found in token" });
            }

            var shippingAddress = _mapper.Map<ShippingAddress>(orderDto.shipToAddress);

            var order = await _orderService.CreateOrderAsync(userEmail, basketId, orderDto.DeliveryMethodId, shippingAddress);

            if (order == null)
            {
                return BadRequest(new { Message = "There are no items in the customer basket, so we can't create the order" });
            }

            var orderToReturn = _mapper.Map<OrderToReturnDto>(order);
            return Ok(new { Message = "Order created successfully", Order = orderToReturn });
        }
        [ProducesResponseType(typeof(IReadOnlyList<OrderToReturnDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("user-orders")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(BuyerEmail))
            {
                return BadRequest(new ApiResponse(400, "Invalid buyer email"));
            }

            var Orders = await _orderService.CreateOrderForSpecUserAsync(BuyerEmail);
            if (Orders == null || Orders.Count == 0)
            {
                return NotFound(new ApiResponse(404, "There are no orders found"));
            }

            var MappedOrders = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(Orders);
            return Ok(MappedOrders);
        }

        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("user-orders/{orderId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int orderId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { Message = "User email not found in token" });
            }

            var order = await _orderService.CreateOrderByIdForSpecUserAsync(userEmail, orderId);

            if (order == null)
                return NotFound(new ApiResponse(404, "Order not found for the user"));

            var orderToReturn = _mapper.Map<OrderToReturnDto>(order);
            return Ok(orderToReturn);
        }


        [HttpGet("DeliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            var deliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
            return Ok(deliveryMethods);
        }
        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderById(int orderId)
        {
           

            var order = await _orderService.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return NotFound(new ApiResponse(404, "Order not found"));
            }


            var orderToReturn = _mapper.Map<OrderToReturnDto>(order);
            return Ok(orderToReturn);
        }
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrderById(int orderId)
        {
           

            var order = await _orderService.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return NotFound(new ApiResponse(404, "Order not found"));
            }

            //if (order.BuyerEmail != userEmail)
            //{
            //    return Unauthorized(new { Message = "You are not authorized to delete this order" });
            //}

            await _orderService.DeleteOrderAsync(orderId);

            return Ok(new { Message = "Order deleted successfully" });
        }
        [HttpGet("all-orders")]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            if (orders == null || orders.Count == 0)
            {
                return NotFound(new ApiResponse(404, "No orders found"));
            }

            var mappedOrders = _mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders);
            return Ok(mappedOrders);
        }

    }
}
