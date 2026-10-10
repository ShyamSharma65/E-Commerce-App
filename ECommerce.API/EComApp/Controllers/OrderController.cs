using System.Security.Claims;
using ECommerceAPI.Application.DTOs;
using ECommerceAPI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Customer")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult<OrderSummaryDto>> Checkout([FromBody] CheckoutOrderRequestDto request)
        {
            var customerId = GetCustomerId();
            var order = await _orderService.CheckoutAsync(customerId, request);
            return Ok(order);
        }

        [HttpGet("my-orders")]
        public async Task<ActionResult<List<OrderSummaryDto>>> GetMyOrders()
        {
            var customerId = GetCustomerId();
            var orders = await _orderService.GetMyOrdersAsync(customerId);
            return Ok(orders);
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
