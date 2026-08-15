# Heritage Marketplace

An online marketplace for heritage-inspired products — home & decoration, accessories, phone
covers, traditional wear, and heritage books representing the culture of countries around the
world. Built for the COMP420 (Application Development) course project.

## Tech Stack

- ASP.NET Core 8 MVC (C#)
- Entity Framework Core 8, Code First, SQL Server
- ASP.NET Core Identity (cookie auth for the site, JWT bearer for the REST API)
- Serilog (structured logging), Swagger / Swashbuckle (API docs)
- Bootstrap 5, Chart.js
- xUnit + Moq + EF Core InMemory (unit tests)

## Architecture

N-Layer solution:

```
HeritageMarket.Domain          entities, enums, repository/UoW interfaces (no dependencies)
HeritageMarket.Application     DTOs, service interfaces & implementations (business logic)
HeritageMarket.Infrastructure  EF Core DbContext, repositories, Identity, JWT, background service
HeritageMarket.Web             MVC controllers/views, Admin area, REST API controllers
HeritageMarket.Tests           xUnit test suite for the Application service layer
```

`Web` depends on `Application` + `Infrastructure`; `Infrastructure` depends on `Domain` +
`Application`; `Application` depends only on `Domain`. Data access goes through a generic
`IRepository<T>` plus a few entity-specific repositories, coordinated by `IUnitOfWork`.

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB is fine — the default connection string targets
  `(localdb)\mssqllocaldb`)

### Run it

```powershell
git clone <this-repo>
cd Application.net
dotnet build

# Apply migrations & run (the app also seeds roles/admin/catalog data on first run)
dotnet run --project src/HeritageMarket.Web
```

The app seeds automatically on startup (`DbInitializer`) — no manual `dotnet ef database update`
is required, though the migration lives at
`src/HeritageMarket.Infrastructure/Persistence/Migrations`.

Open `https://localhost:<port>` (see console output for the exact port). Swagger UI is available
at `/swagger`.

### Seeded accounts

| Role | Email | Password |
|---|---|---|
| Admin | `admin@heritagemarket.local` | `Admin@12345` |

Register any other account through the UI — it's assigned the `Customer` role automatically.

### Connection string / secrets

`src/HeritageMarket.Web/appsettings.json` holds the default LocalDB connection string and a
placeholder JWT signing key. For anything beyond local development, override both via
`appsettings.Development.json`, environment variables, or `dotnet user-secrets`.

## Running Tests

```powershell
dotnet test
```

Tests exercise the Application service layer (`ProductService`, `CartService`, `OrderService`,
`ReportService`) against a real `ApplicationDbContext` backed by the EF Core InMemory provider —
no SQL Server instance required.

## REST API

Public read endpoints (no auth): `GET /api/products`, `GET /api/products/{id}`,
`GET /api/categories`, `GET /api/countries`.

Authenticated endpoints (JWT bearer, obtained from `POST /api/auth/login`):
`GET/POST/PUT/DELETE /api/cart/...`, `GET/POST /api/orders`.

Full interactive documentation: `/swagger`.

## Background Service

`LowStockNotificationService` (Infrastructure) runs on an interval (default: hourly, configurable
via `LowStock:CheckIntervalMinutes` in `appsettings.json`) and creates a `Notification` when a
product's stock drops to or below `LowStock:ThresholdQuantity` (default: 5). Notifications surface
on the Admin dashboard.

## Documentation

The `docs/` folder contains the course-required deliverables:

- [ProjectProposal.md](docs/ProjectProposal.md)
- [TechnicalDocumentation.md](docs/TechnicalDocumentation.md) — architecture, API, DB schema, security, testing
- [Diagrams.md](docs/Diagrams.md) — ER diagram, system architecture diagram, use case diagram (Mermaid)
- [Presentation.md](docs/Presentation.md) — slide-by-slide outline + live demo script

## Project Structure Highlights

- `Areas/Identity/Pages/Account` — custom Login/Register/Forgot-Reset Password/Manage(profile)
  Razor Pages built directly against `UserManager`/`SignInManager` (not the scaffolded default UI),
  so the extra `FullName`/`Address`/`ProfileImageUrl` fields and Bootstrap styling are first-class.
- `Areas/Admin` — the Admin panel: dashboard, Products/Categories/Countries/Orders/Users CRUD, and
  a sales report page with CSV export and a Chart.js chart.
- `Controllers/Api` — the REST API controllers.
- `wwwroot/uploads` — runtime-uploaded product and profile images (gitignored).
