using ECommerceAPI.AppDbContext;
using ECommerceAPI.Application.DTOs;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Application.Services
{
    public interface IOrderService
    {
        Task<List<OrderSummaryDto>> GetMyOrdersAsync(int customerId);
        Task<OrderSummaryDto> CheckoutAsync(int customerId, CheckoutOrderRequestDto request);
    }

    public class OrderService : IOrderService
    {
        private readonly EComDbContext _context;

        public OrderService(EComDbContext context)
        {
            _context = context;
        }

        public async Task<List<OrderSummaryDto>> GetMyOrdersAsync(int customerId)
        {
            var orders = await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(o => new OrderSummaryDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                TotalAmount = o.TotalAmount,
                Status = o.OrderStatus.ToString(),
                CreatedAt = o.OrderDate
            }).ToList();
        }

        public async Task<OrderSummaryDto> CheckoutAsync(int customerId, CheckoutOrderRequestDto request)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && !c.IsCheckedOut);

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                throw new InvalidOperationException("Your cart is empty.");

            var billingAddress = new Address
            {
                CustomerId = customerId,
                Address1 = string.IsNullOrWhiteSpace(request.BillingAddress) ? "Main Street" : request.BillingAddress,
                Address2 = string.Empty,
                State = string.IsNullOrWhiteSpace(request.State) ? "Default" : request.State,
                City = string.IsNullOrWhiteSpace(request.City) ? "Default City" : request.City,
                PostalCode = string.IsNullOrWhiteSpace(request.PostalCode) ? "00000" : request.PostalCode,
                Country = string.IsNullOrWhiteSpace(request.Country) ? "India" : request.Country
            };

            var shippingAddress = new Address
            {
                CustomerId = customerId,
                Address1 = string.IsNullOrWhiteSpace(request.ShippingAddress) ? "Main Street" : request.ShippingAddress,
                Address2 = string.Empty,
                State = string.IsNullOrWhiteSpace(request.State) ? "Default" : request.State,
                City = string.IsNullOrWhiteSpace(request.City) ? "Default City" : request.City,
                PostalCode = string.IsNullOrWhiteSpace(request.PostalCode) ? "00000" : request.PostalCode,
                Country = string.IsNullOrWhiteSpace(request.Country) ? "India" : request.Country
            };

            _context.Addresses.AddRange(billingAddress, shippingAddress);
            await _context.SaveChangesAsync();

            var totalBaseAmount = cart.CartItems.Sum(item => item.TotalPrice);
            var order = new Order
            {
                OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{customerId}",
                CustomerId = customerId,
                BillingAddressId = billingAddress.Id,
                ShippingAddressId = shippingAddress.Id,
                TotalBaseAmount = totalBaseAmount,
                TotalDiscountAmount = 0,
                ShippingCost = 0,
                TotalAmount = totalBaseAmount,
                OrderStatus = OrderStatus.Pending,
                OrderDate = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in cart.CartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = item.Discount,
                    TotalPrice = item.TotalPrice
                });
            }

            _context.Orders.Add(order);
            cart.IsCheckedOut = true;
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new OrderSummaryDto
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                TotalAmount = order.TotalAmount,
                Status = order.OrderStatus.ToString(),
                CreatedAt = order.OrderDate
            };
        }
    }
}
