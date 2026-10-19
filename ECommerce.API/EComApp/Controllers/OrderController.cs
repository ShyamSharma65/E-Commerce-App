using System.Security.Claims;
using ECommerceAPI.Application.DTOs;
using ECommerceAPI.Application.Services;
using ECommerceAPI.DTOs;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<ActionResult<OrderSummaryDto>> Checkout([FromBody] CheckoutOrderRequestDto request)
        {
            var customerId = GetCustomerId();
            var order = await _orderService.CheckoutAsync(customerId, request);
            return Ok(order);
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("my-orders")]
        public async Task<ActionResult<List<OrderSummaryDto>>> GetMyOrders()
        {
            var customerId = GetCustomerId();
            var orders = await _orderService.GetMyOrdersAsync(customerId);
            return Ok(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        [ProducesResponseType(typeof(List<OrderSummaryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<OrderSummaryDto>>> GetAllOrders(CancellationToken cancellationToken)
        {
            var orders = await _orderService.GetAllOrdersAsync(cancellationToken);
            return Ok(orders);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{orderId:int}/status")]
        [ProducesResponseType(typeof(OrderSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderSummaryDto>> UpdateOrderStatus(
            int orderId,
            [FromBody] UpdateOrderStatusRequestDto request,
            CancellationToken cancellationToken)
        {
            if (!Enum.IsDefined(request.Status))
            {
                return BadRequest(new { message = "Invalid order status." });
            }

            var order = await _orderService.UpdateOrderStatusAsync(orderId, request.Status, cancellationToken);
            return order is null ? NotFound() : Ok(order);
        }

        private int GetCustomerId()
        {
            var claim = User.FindFirstValue("customerId");
            if (string.IsNullOrWhiteSpace(claim) || !int.TryParse(claim, out var customerId))
            {
                throw new UnauthorizedAccessException("Invalid customer identity.");
            }

            return customerId;
        }
    }
}
