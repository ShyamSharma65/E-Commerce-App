# E-Commerce App Fullstack

This repository contains a fullstack e-commerce application with an Angular frontend and an ASP.NET Core backend.

## Project Overview

- Frontend: `ECommerce.UI` — Angular 20 application with SSR support.
- Backend: `ECommerce.API/EComApp` — ASP.NET Core 8 Web API using Entity Framework Core and SQL Server.
- Current scope: customer registration and account update flow are implemented in the backend and frontend.
- Some app features are already added, while other ecommerce workflows are present as data models or planned features.

## Key Features Implemented

- Customer registration with form validation
- Customer account update endpoint
- Angular routing for home, login, and register pages
- Backend service support for customer login, registration, update, password change, and customer details retrieval
- EF Core data models for:
  - Customers
  - Addresses
  - Categories
  - Products
  - Carts and CartItems
  - Orders and OrderItems
  - Payments
  - Cancellations and Refunds
  - Feedback
- Seed data for product categories, products, and status values
- Swagger enabled for backend API exploration in development

## Pending / Planned Features

- Expose backend routes for customer login, password change, and customer details retrieval
- Cart and checkout APIs and UI integration
- Product listing and product details pages
- Order history and order placement workflows
- Payment processing flow
- Cancellation and refund workflows
- Feedback/review submission UI and backend support
- Authentication/session management and route protection
- Complete customer profile page routing and frontend integration

## Folder Structure

- `ECommerce.API/EComApp`
  - `Controllers` — API controllers
  - `Services` — business logic implementations and interfaces
  - `AppDbContext` — EF Core database context and data seeding
  - `Models` — entity definitions
  - `DTOs` — request/response contracts
  - `Migrations` — EF Core database migrations
- `ECommerce.UI`
  - `src/app` — Angular app components and routing
  - `src/app/customer` — customer-facing pages and service
  - `package.json` — Angular and npm dependencies

## Running the Backend

1. Open `ECommerce.API/EComApp` in a terminal.
2. Update the connection string in `appsettings.json` if needed:

```json
"ConnectionStrings": {
  "EComDevConnectionString": "Server=localhost\\SQLEXPRESS;Database=ECommerceAppDB;Trusted_Connection=True;TrustServerCertificate=true;"
}
```

3. Restore dependencies and update the database:

```bash
dotnet restore
dotnet ef database update
```

4. Run the API:

```bash
dotnet run
```

5. In development, Swagger UI is available at `https://localhost:<port>/swagger`.

## Running the Frontend

1. Open `ECommerce.UI` in a terminal.
2. Install dependencies:

```bash
npm install
```

3. Start the Angular development server:

```bash
npm start
```

4. Open the application at `http://localhost:4200/`.

## Notes

- The frontend currently uses a hard-coded backend endpoint in `src/app/customer/service/cutomerservice.ts`:
  `https://localhost:7287/customer/registercustomer`
- If the backend runs on a different port, update that URL or add configuration support.
- The current Angular routes include ``, `login`, and `register`.
- Additional pages such as profile details, cart, product listing, and order management are available in the codebase structure, but they need further routing and API integration.

## Suggested Next Steps

- Add API controllers for login, get customer details, change password, cart, order, payment, refund, and feedback.
- Build the frontend product catalog, shopping cart, checkout, and customer profile pages.
- Add authentication token handling and protected routes.
- Connect the seeded product data to a product listing page.
- Implement end-to-end flows for placing orders and requesting refunds.
