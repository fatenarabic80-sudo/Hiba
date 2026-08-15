# Technical Documentation — Heritage Marketplace

## 1. System Architecture Overview

Heritage Marketplace follows an **N-Layer architecture** with strict one-directional dependencies,
implemented as five .NET projects in a single solution:

```
HeritageMarket.Domain            entities, enums, repository/UoW interfaces — no dependencies
HeritageMarket.Application        DTOs, service interfaces & implementations (business logic)
HeritageMarket.Infrastructure     EF Core DbContext, repositories, Identity, JWT, background service
HeritageMarket.Web                MVC controllers/views, Admin area, REST API controllers
HeritageMarket.Tests              xUnit test suite for the Application service layer
```

Dependency direction: `Web → Application, Infrastructure → Application, Domain`. `Application`
depends only on `Domain`, and never references EF Core's relational/SQL Server packages directly —
only the core `Microsoft.EntityFrameworkCore` package, for its async LINQ operators
(`ToListAsync`, `FirstOrDefaultAsync`, etc.) over the `IQueryable<T>` exposed by
`IRepository<T>.Query()`. This keeps business logic testable against any `IUnitOfWork`
implementation (production: EF Core + SQL Server; tests: EF Core InMemory).

See [Diagrams.md](Diagrams.md) for the visual system architecture diagram.

### Design patterns used

- **Repository pattern** — `IRepository<T>` (generic CRUD + `Query()`) plus entity-specific
  repositories (`IProductRepository`, `ICartRepository`, `IOrderRepository`) for queries that need
  extra `Include()`s.
- **Unit of Work** — `IUnitOfWork` exposes all repositories and a single `SaveChangesAsync()`,
  guaranteeing one `DbContext` / one transaction per business operation.
- **Dependency Injection** — every service, repository, and cross-cutting concern (logging, token
  generation, user lookups) is registered and injected via the built-in ASP.NET Core DI container
  (`AddApplicationServices()` / `AddInfrastructure()` extension methods).
- **DTO projection** — all query results are projected directly into DTOs
  (`Application/DTOs`) via translatable `Expression<Func<TEntity, TDto>>` fields, so EF Core
  generates a single SQL query with joins/aggregates instead of hydrating full entity graphs.

## 2. Authentication & Authorization

- **ASP.NET Core Identity** with a custom `ApplicationUser` (extends `IdentityUser` with
  `FullName`, `Address`, `ProfileImageUrl`, `CreatedAt`) and two roles: `Admin`, `Customer`.
- The MVC site uses **cookie authentication** (Identity's default scheme). Admin-area controllers
  and views are protected with `[Authorize(Roles = "Admin")]`.
- The REST API uses a **separate JWT Bearer scheme**, registered alongside the cookie scheme in
  `Program.cs`. `POST /api/auth/login` validates credentials via `UserManager`/`SignInManager` and
  issues a signed JWT (`HeritageMarket.Infrastructure.Auth.TokenService`) carrying the user's id,
  email, and role claims. API controllers that require auth use
  `[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]`.
- Passwords are hashed by Identity's `PasswordHasher`; lockout is enabled after 5 failed attempts.

## 3. API Documentation

Full interactive documentation (request/response schemas, try-it-out) is served by Swagger at
`/swagger` when the app is running. Summary of the REST surface:

| Endpoint | Auth | Description |
|---|---|---|
| `POST /api/auth/login` | none | Authenticates and returns a JWT |
| `GET /api/products` | none | Paged/filterable product catalog |
| `GET /api/products/{id}` | none | Product detail + reviews |
| `GET /api/categories` | none | All categories |
| `GET /api/countries` | none | All countries |
| `GET /api/cart` | JWT | The authenticated user's cart |
| `POST /api/cart/items` | JWT | Add an item to the cart |
| `PUT /api/cart/items/{id}` | JWT | Update a cart item's quantity |
| `DELETE /api/cart/items/{id}` | JWT | Remove a cart item |
| `GET /api/orders` | JWT | The authenticated user's orders |
| `GET /api/orders/{id}` | JWT | A single order (must belong to the caller) |
| `POST /api/orders` | JWT | Place an order from the current cart |

## 4. Database Schema

SQL Server, EF Core Code First. Primary keys are `int Id` (identity) on every table except the
Identity tables, which use `string Id` (GUID) per ASP.NET Core Identity convention.

| Table | Key columns | Notes |
|---|---|---|
| `AspNetUsers` (ApplicationUser) | `FullName`, `Address`, `ProfileImageUrl`, `CreatedAt` | extends Identity's user table |
| `AspNetRoles` | `Admin`, `Customer` | seeded roles |
| `Countries` | `Name` (unique), `Code`, `FlagImageUrl`, `Description` | |
| `Categories` | `Name` (unique), `Description`, `IconUrl` | |
| `Products` | `Name`, `Description`, `Price` (decimal 18,2), `StockQuantity`, `ImageUrl`, `SKU` (unique), `IsActive`, `CreatedAt`, `CategoryId` (FK, Restrict), `CountryId` (FK, Restrict) | |
| `Carts` | `ApplicationUserId` (FK, unique — one cart per user) | |
| `CartItems` | `CartId` (FK, Cascade), `ProductId` (FK, Cascade), `Quantity` | |
| `Orders` | `ApplicationUserId` (FK, Restrict), `OrderDate`, `Status` (enum), `TotalAmount`, `ShippingAddress`, `ShippingCity` | |
| `OrderItems` | `OrderId` (FK, Cascade), `ProductId` (FK, Restrict), `Quantity`, `UnitPrice` (price snapshot) | |
| `Reviews` | `ProductId` (FK, Cascade), `ApplicationUserId` (FK, Cascade), `Rating` (1–5), `Comment`, `CreatedAt` | |
| `Notifications` | `Message`, `CreatedAt`, `IsRead`, `ProductId` (FK, SetNull, nullable) | written by the low-stock background service |

See [Diagrams.md](Diagrams.md) for the entity-relationship diagram.

## 5. Logging & Exception Handling

- **Serilog** writes structured logs to the console and to rolling daily files under
  `src/HeritageMarket.Web/Logs/`. `UseSerilogRequestLogging()` logs one line per HTTP request.
- A custom `ExceptionHandlingMiddleware` catches unhandled exceptions, logs them, and returns:
  - a JSON `problem`-style body for `/api/*` requests or `Accept: application/json`, or
  - a redirect to a friendly `Error`/`NotFound` view for regular page requests.
- Domain-level failures (`NotFoundException`, `InsufficientStockException`) map to 404/400 instead
  of a generic 500.

## 6. Background Service

`LowStockNotificationService` (`Infrastructure/BackgroundServices`) is a `BackgroundService`
registered via `AddHostedService`. On a configurable interval (`LowStock:CheckIntervalMinutes` in
`appsettings.json`, default 60 minutes) it scans for active products at or below
`LowStock:ThresholdQuantity` (default 5) and writes a `Notification`, de-duplicated so the same
product isn't re-flagged more than once per 24 hours. Notifications are surfaced on the Admin
dashboard.

## 7. Security Measures

- Anti-forgery tokens on every state-changing form (`[ValidateAntiForgeryToken]` + Razor's
  `FormTagHelper`, which injects the token automatically)
- Identity password policy (min length 8) + account lockout after repeated failed logins
- HTTPS redirection + HSTS in non-development environments
- Razor's automatic output encoding (XSS protection) and EF Core's parameterized queries (SQL
  injection protection) — no raw SQL string concatenation anywhere in the codebase
- Upload validation on all `IFormFile` inputs: extension whitelist (`.jpg/.jpeg/.png/.webp`), 5 MB
  size cap, and server-generated (GUID) filenames to prevent path traversal
- Short-lived, signed JWTs for the API surface, separate from the site's session cookie

## 8. Testing

`HeritageMarket.Tests` (xUnit + Moq) exercises the Application service layer
(`ProductService`, `CartService`, `OrderService`, `ReportService`) against a real
`ApplicationDbContext` backed by the EF Core **InMemory** provider — this validates actual LINQ
query translation (catching, for example, projections that can't be translated to SQL) without
requiring a live SQL Server instance. Run with `dotnet test`.
