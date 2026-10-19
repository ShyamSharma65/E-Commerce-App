using System.ComponentModel.DataAnnotations;

namespace ECommerce.CatalogService;

/// <summary>Payload used by administrators to add a catalog product.</summary>
public sealed record CreateCatalogProductRequest
{
    [Required, StringLength(100, MinimumLength = 3)]
    public required string Name { get; init; }

    [Required, MinLength(10)]
    public required string Description { get; init; }

    [Range(typeof(decimal), "0.01", "10000.00")]
    public decimal Price { get; init; }

    [Range(0, 1000)]
    public int StockQuantity { get; init; }

    [Required, StringLength(2048)]
    public required string ImageUrl { get; init; }

    [Range(0, 100)]
    public int DiscountPercentage { get; init; }

    public bool IsAvailable { get; init; } = true;

    [Range(1, int.MaxValue)]
    public int CategoryId { get; init; }
}

/// <summary>Catalog product data returned by the API.</summary>
public sealed record CatalogProductResponse(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int StockQuantity,
    string ImageUrl,
    int DiscountPercentage,
    bool IsAvailable,
    int CategoryId,
    string CategoryName);

/// <summary>A product category available in the catalog.</summary>
public sealed record CatalogCategoryResponse(int Id, string Name);