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
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<CartSummaryDto>> GetCart()
        {
            var customerId = GetCustomerId();
            var cart = await _cartService.GetCartAsync(customerId);
            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<ActionResult<CartSummaryDto>> AddItem([FromBody] AddToCartRequestDto request)
        {
            var customerId = GetCustomerId();
            var cart = await _cartService.AddItemAsync(customerId, request);
            return Ok(cart);
        }

        [HttpDelete("items/{productId:int}")]
        public async Task<ActionResult<CartSummaryDto>> RemoveItem(int productId)
        {
            var customerId = GetCustomerId();
            var cart = await _cartService.RemoveItemAsync(customerId, productId);
            return Ok(cart);
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
