# API Architecture

## Current topology

Clients should use the YARP gateway at `http://localhost:5000`. Product listing (`GET /api/product`) is routed to the Catalog service at `http://localhost:5180`; all other paths are forwarded to the existing API at `http://localhost:5178`. Both destinations can be changed with the corresponding `ReverseProxy__Clusters__...__Address` environment variables.

```text
							  +-> ECommerce.CatalogService -> ECommerceCatalogDB
Client -> ECommerce.Gateway -+
							  +-> ECommerce.Api -> ECommerceAppDB
```

Catalog now has an independent host, EF Core context, migration, and database. Its initial migration seeds the same three development products as the existing API. To carry over additional catalog rows, apply the Catalog migration and run `ECommerce.CatalogService/Migrations/ImportCatalogData.sql` once against the SQL Server instance. The import is idempotent and copies existing category/product IDs and fields.

## Admin operations

- `POST /api/product` creates a Catalog product. Requires a bearer token with the `Admin` role. The body includes `name`, `description`, `price`, `stockQuantity`, `imageUrl`, `discountPercentage`, `isAvailable`, and `categoryId`.
- `GET /api/product/{id}` returns the created product and is used as the `Location` target for a successful create (`201 Created`).
- `GET /api/category` returns the categories used by the admin product form.
- `GET /api/order/all` returns all orders for the admin order workspace. Requires a bearer token with the `Admin` role.
- `PATCH /api/order/{orderId}/status` updates an order status in the legacy API. Requires a bearer token with the `Admin` role. Example body: `{ "status": "Shipped" }`. Supported values are `Pending`, `Processing`, `Shipped`, `Delivered`, and `Canceled`.

The Angular admin workspace is available at `/admin`; it is shown after Admin sign-in and protected by a client-side route guard. The API role checks remain authoritative.

Public registration only issues `Customer` role tokens. Provision `Admin` through a trusted database/admin process by assigning the customer's `Role` in `ECommerceAppDB`, then have the user sign in again to receive a token containing the new role. Configure matching `Jwt:Key`, `Jwt:Issuer`, and `Jwt:Audience` values for both API hosts in each environment.

An authenticated Admin can also create another Admin from the Angular Register page. The page only shows its account-role selector to Admins, and `POST /api/auth/register-admin` independently enforces the Admin role. Public registration remains Customer-only.

This is still an incremental migration, not a completed domain decomposition. The legacy API retains its own catalog tables because cart and checkout workflows still use its data. Cart lines snapshot name/price when added, but product maintenance and stock validation do not yet have a cross-service contract. Avoid changing products through direct SQL in either database after import; the next step is to move catalog writes to the Catalog service and have cart/order workflows call it or consume catalog events. Customers, carts, and orders remain in the legacy API and database.

## Next extraction boundary

Before extracting a domain service, remove its cross-domain database foreign keys and define the service contract and data migration. Then give that service its own database and route only its owned paths through the gateway. Keep the existing API as the compatibility destination for paths that have not migrated yet.

## Local run

Apply the Catalog schema and import existing catalog rows once:

```powershell
dotnet ef database update --project ECommerce.CatalogService --startup-project ECommerce.CatalogService
sqlcmd -S localhost\SQLEXPRESS -d ECommerceCatalogDB -E -i ECommerce.CatalogService\Migrations\ImportCatalogData.sql
```

Then start each host in a separate terminal:

```powershell
dotnet run --project EComApp --launch-profile http
dotnet run --project ECommerce.CatalogService --launch-profile http
dotnet run --project ECommerce.Gateway
```

Use `http://localhost:5000` as the API base URL. The existing API Swagger endpoint is available through the gateway at `/swagger`; Catalog Swagger and health are available directly at `http://localhost:5180/swagger` and `http://localhost:5180/health`.