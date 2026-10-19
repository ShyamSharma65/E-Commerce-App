using ECommerce.CatalogService;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogConnectionString")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var jwtKey = builder.Configuration["Jwt:Key"] ?? "ECommerceApp_Super_Secret_Key_2026_!@#$%^&*()";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ECommerceApi";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ECommerceClient";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization(options =>
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", service = "catalog" }));

app.MapGet("/api/category", async Task<Ok<List<CatalogCategoryResponse>>> (
    CatalogDbContext db, CancellationToken cancellationToken) =>
{
    var categories = await db.Categories
        .AsNoTracking()
        .OrderBy(category => category.Name)
        .Select(category => new CatalogCategoryResponse(category.Id, category.Name))
        .ToListAsync(cancellationToken);

    return TypedResults.Ok(categories);
});

app.MapGet("/api/product", async Task<Ok<List<CatalogProductResponse>>> (CatalogDbContext db, CancellationToken cancellationToken) =>
{
    var products = await db.Products
        .AsNoTracking()
        .Include(product => product.Category)
        .OrderBy(product => product.Id)
        .Select(product => new CatalogProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.StockQuantity,
            product.ImageUrl,
            product.DiscountPercentage,
            product.IsAvailable,
            product.CategoryId,
            product.Category == null ? "General" : product.Category.Name))
        .ToListAsync(cancellationToken);

    return TypedResults.Ok(products);
});

app.MapGet("/api/product/{id:int}", async Task<Results<Ok<CatalogProductResponse>, NotFound>> (
    int id, CatalogDbContext db, CancellationToken cancellationToken) =>
{
    var product = await db.Products
        .AsNoTracking()
        .Where(product => product.Id == id)
        .Select(product => new CatalogProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.StockQuantity,
            product.ImageUrl,
            product.DiscountPercentage,
            product.IsAvailable,
            product.CategoryId,
            product.Category == null ? "General" : product.Category.Name))
        .FirstOrDefaultAsync(cancellationToken);

    return product is null ? TypedResults.NotFound() : TypedResults.Ok(product);
});

app.MapPost("/api/product", async Task<Results<Created<CatalogProductResponse>, ValidationProblem, NotFound<string>>> (
    CreateCatalogProductRequest request, CatalogDbContext db, CancellationToken cancellationToken) =>
{
    var validationResults = new List<ValidationResult>();
    if (!Validator.TryValidateObject(request, new ValidationContext(request), validationResults, validateAllProperties: true))
    {
        var errors = validationResults
            .SelectMany(result => result.MemberNames.Select(member => new { member, result.ErrorMessage }))
            .GroupBy(error => error.member)
            .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage ?? "Invalid value.").ToArray());

        return TypedResults.ValidationProblem(errors);
    }

    var category = await db.Categories
        .FirstOrDefaultAsync(category => category.Id == request.CategoryId, cancellationToken);
    if (category is null)
    {
        return TypedResults.NotFound("Category not found.");
    }

    var product = new CatalogProduct
    {
        Name = request.Name,
        Description = request.Description,
        Price = request.Price,
        StockQuantity = request.StockQuantity,
        ImageUrl = request.ImageUrl,
        DiscountPercentage = request.DiscountPercentage,
        IsAvailable = request.IsAvailable,
        CategoryId = request.CategoryId,
        Category = category
    };

    db.Products.Add(product);
    await db.SaveChangesAsync(cancellationToken);

    var response = new CatalogProductResponse(
        product.Id,
        product.Name,
        product.Description,
        product.Price,
        product.StockQuantity,
        product.ImageUrl,
        product.DiscountPercentage,
        product.IsAvailable,
        product.CategoryId,
        category.Name);

    return TypedResults.Created($"/api/product/{product.Id}", response);
})
    .RequireAuthorization("AdminOnly");

app.Run();