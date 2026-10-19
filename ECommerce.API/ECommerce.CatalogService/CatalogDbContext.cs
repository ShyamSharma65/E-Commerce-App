using Microsoft.EntityFrameworkCore;

namespace ECommerce.CatalogService;

public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<CatalogCategory> Categories => Set<CatalogCategory>();
    public DbSet<CatalogProduct> Products => Set<CatalogProduct>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatalogProduct>()
            .Property(product => product.Price)
            .HasPrecision(18, 2);
        modelBuilder.Entity<CatalogProduct>()
            .Property(product => product.IsAvailable)
            .HasDefaultValue(true);

        modelBuilder.Entity<CatalogCategory>().HasData(
            new CatalogCategory { Id = 1, Name = "Electronics" },
            new CatalogCategory { Id = 2, Name = "Books" });

        modelBuilder.Entity<CatalogProduct>().HasData(
            new CatalogProduct
            {
                Id = 1,
                Name = "Smartphone",
                Description = "Latest model smartphone with advanced features.",
                Price = 699.99m,
                StockQuantity = 50,
                ImageUrl = "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?auto=format&fit=crop&w=800&q=80",
                DiscountPercentage = 10,
                IsAvailable = true,
                CategoryId = 1
            },
            new CatalogProduct
            {
                Id = 2,
                Name = "Laptop",
                Description = "High-performance laptop suitable for all your needs.",
                Price = 999.99m,
                StockQuantity = 30,
                ImageUrl = "https://images.unsplash.com/photo-1496181133206-80ce9b88a853?auto=format&fit=crop&w=800&q=80",
                DiscountPercentage = 15,
                IsAvailable = true,
                CategoryId = 1
            },
            new CatalogProduct
            {
                Id = 3,
                Name = "Science Fiction Novel",
                Description = "A thrilling science fiction novel set in the future.",
                Price = 19.99m,
                StockQuantity = 100,
                ImageUrl = "https://images.unsplash.com/photo-1512820790803-83ca734da794?auto=format&fit=crop&w=800&q=80",
                DiscountPercentage = 5,
                IsAvailable = true,
                CategoryId = 2
            });
    }
}

public sealed class CatalogCategory
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<CatalogProduct> Products { get; set; } = [];
}

public sealed class CatalogProduct
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public required string ImageUrl { get; set; }
    public int DiscountPercentage { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int CategoryId { get; set; }
    public CatalogCategory? Category { get; set; }
}