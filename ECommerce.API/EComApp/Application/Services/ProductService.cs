using ECommerceAPI.AppDbContext;
using ECommerceAPI.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Application.Services
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllProductsAsync();
    }

    public class ProductService : IProductService
    {
        private readonly EComDbContext _context;

        public ProductService(EComDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Id)
                .ToListAsync();

            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ImageUrl = GetProductImageUrl(p),
                CategoryName = p.Category != null ? p.Category.Name : "General"
            }).ToList();
        }

        private static string GetProductImageUrl(ECommerceAPI.Models.Product product)
        {
            if (!string.IsNullOrWhiteSpace(product.ImageUrl))
            {
                return product.ImageUrl;
            }

            return product.Id switch
            {
                1 => "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?auto=format&fit=crop&w=800&q=80",
                2 => "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?auto=format&fit=crop&w=800&q=80",
                3 => "https://images.unsplash.com/photo-1512820790803-83ca734da794?auto=format&fit=crop&w=800&q=80",
                _ => "https://images.unsplash.com/photo-1524758631624-e2822e304c36?auto=format&fit=crop&w=800&q=80"
            };
        }
    }
}
