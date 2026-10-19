SET XACT_ABORT ON;
BEGIN TRANSACTION;

SET IDENTITY_INSERT dbo.Categories ON;
UPDATE target
SET target.Name = source.Name
FROM dbo.Categories AS target
INNER JOIN ECommerceAppDB.dbo.Categories AS source ON source.Id = target.Id;

INSERT INTO dbo.Categories (Id, Name)
SELECT source.Id, source.Name
FROM ECommerceAppDB.dbo.Categories AS source
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.Categories AS target
    WHERE target.Id = source.Id
);
SET IDENTITY_INSERT dbo.Categories OFF;

SET IDENTITY_INSERT dbo.Products ON;
UPDATE target
SET target.Name = source.Name,
    target.Description = source.Description,
    target.Price = source.Price,
    target.StockQuantity = source.StockQuantity,
    target.ImageUrl = source.ImageUrl,
    target.DiscountPercentage = source.DiscountPercentage,
    target.IsAvailable = source.IsAvailable,
    target.CategoryId = source.CategoryId
FROM dbo.Products AS target
INNER JOIN ECommerceAppDB.dbo.Products AS source ON source.Id = target.Id;

INSERT INTO dbo.Products (Id, Name, Description, Price, StockQuantity, ImageUrl, DiscountPercentage, IsAvailable, CategoryId)
SELECT source.Id, source.Name, source.Description, source.Price, source.StockQuantity, source.ImageUrl, source.DiscountPercentage, source.IsAvailable, source.CategoryId
FROM ECommerceAppDB.dbo.Products AS source
WHERE NOT EXISTS (
    SELECT 1
    FROM dbo.Products AS target
    WHERE target.Id = source.Id
);
SET IDENTITY_INSERT dbo.Products OFF;

COMMIT TRANSACTION;