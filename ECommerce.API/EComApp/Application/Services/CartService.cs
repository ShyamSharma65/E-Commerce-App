using ECommerceAPI.AppDbContext;
using ECommerceAPI.Application.DTOs;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Application.Services
{
    public interface ICartService
    {
        Task<CartSummaryDto> GetCartAsync(int customerId);
        Task<CartSummaryDto> AddItemAsync(int customerId, AddToCartRequestDto request);
        Task<CartSummaryDto> RemoveItemAsync(int customerId, int productId);
    }

    public class CartService : ICartService
    {
        private readonly EComDbContext _context;

        public CartService(EComDbContext context)
        {
            _context = context;
        }

        public async Task<CartSummaryDto> GetCartAsync(int customerId)
        {
            var cart = await GetOrCreateCartAsync(customerId);
            return MapCart(cart);
        }

        public async Task<CartSummaryDto> AddItemAsync(int customerId, AddToCartRequestDto request)
        {
            if (request.Quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero.");

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId);
            if (product == null)
                throw new InvalidOperationException("Product not found.");

            var cart = await GetOrCreateCartAsync(customerId);
            var existingItem = cart.CartItems.FirstOrDefault(item => item.ProductId == request.ProductId);

            if (existingItem == null)
            {
                var item = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    UnitPrice = product.Price,
                    Discount = 0,
                    TotalPrice = product.Price * request.Quantity,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.CartItems.Add(item);
            }
            else
            {
                existingItem.Quantity += request.Quantity;
                existingItem.UnitPrice = product.Price;
                existingItem.TotalPrice = product.Price * existingItem.Quantity;
                existingItem.UpdatedAt = DateTime.UtcNow;
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetCartAsync(customerId);
        }

        public async Task<CartSummaryDto> RemoveItemAsync(int customerId, int productId)
        {
            var cart = await GetOrCreateCartAsync(customerId);
            var item = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
                throw new InvalidOperationException("Item not found in cart.");

            _context.CartItems.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetCartAsync(customerId);
        }

        private async Task<Cart> GetOrCreateCartAsync(int customerId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && !c.IsCheckedOut);

            if (cart != null)
                return cart;

            var newCart = new Cart
            {
                CustomerId = customerId,
                IsCheckedOut = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CartItems = new List<CartItem>()
            };

            _context.Carts.Add(newCart);
            await _context.SaveChangesAsync();

            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(item => item.Product)
                .FirstAsync(c => c.Id == newCart.Id);
        }

        private static CartSummaryDto MapCart(Cart cart)
        {
            var items = cart.CartItems?
                .Select(item => new CartItemDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? "Product",
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                })
                .ToList() ?? new List<CartItemDto>();

            return new CartSummaryDto
            {
                CartId = cart.Id,
                Items = items,
                TotalAmount = items.Sum(item => item.TotalPrice),
                TotalItems = items.Sum(item => item.Quantity)
            };
        }
    }
}
