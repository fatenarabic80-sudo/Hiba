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

| Role | Sign in at | Email | Password |
|---|---|---|---|
| Admin | `/Admin/Account/Login` | `admin@heritagemarket.local` | `Admin@12345` |

Register any other account through the UI (`/Identity/Account/Register`) — it's assigned the
`Customer` role automatically.

### Admin authentication is separate from customer authentication

Admins do **not** sign in through the same form as customers:

- Customers sign in at `/Identity/Account/Login`. If those credentials belong to an Admin, the
  form rejects them with a message pointing to the Admin login instead of authenticating them.
- Admins sign in at `/Admin/Account/Login` (a distinct, unbranded portal page — not linked from the
  public site nav). If those credentials belong to a Customer, this form rejects them too.
- Any anonymous request into `/Admin/*` is redirected to `/Admin/Account/Login`, never the
  customer login page (see the `OnRedirectToLogin` override in `Program.cs`).
- The Admin's own account/profile (name, address, password) is managed at `/Admin/Profile` —
  separate from the customer-facing `/Identity/Account/Manage` page.
- `Admin/Users` only ever lists and manages **Customer** accounts — administrator accounts never
  appear there and can't be edited/locked/deleted/promoted from that screen, even by URL
  manipulation (`UsersController` checks the target's role server-side on every action).

Both login forms still share the same underlying ASP.NET Core Identity user store and cookie —
this is one authentication system with two entry points and role-based gating, not two separate
systems.

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

## Catalog

18 countries grouped into 4 heritage regions — **Arab World** (Lebanon, Egypt, Morocco, Palestine,
Syria, Jordan, Iraq, Tunisia), **Asia** (India, Japan, Turkey), **Europe** (France, Italy, Greece,
Spain, Germany, Portugal), **Americas** (United States) — each with 2 real products in every
category (Home & Decoration, Accessories, Phone Covers, Wear & Traditional Clothing) plus 2 real,
well-known books (see below). 180 products total. The Home page and Shop filter group countries by
region (click a region to reveal its countries); `Country.Region` is editable per-country in the
Admin panel. Seed data lives in `Infrastructure/Persistence/Seed/HeritageCatalogSeedData.cs`.

## Heritage Books — a gated category

Every country's 2 books are real, well-known works — several are Nobel laureates or centuries-old
classics (Dante, Homer, Cervantes, Murasaki Shikibu, Tagore, Gibran, Mahfouz, Hugo, Camus, Pamuk,
Goethe, Twain, Morrison, and more) by authors celebrated both at home and internationally.

Heritage Books isn't browsable like the other categories. Clicking it (from the nav, a category
tile, or a direct link) auto-opens the site's real AI chat widget (`Assistant/Ask`, the same one
available everywhere else) and the Heritage Guide drives the whole exchange: if you're not logged
in, it says so and links Login/Register right there in the chat; otherwise it asks which country's
heritage you love (as quick-reply chips — reliable, no text parsing needed) and then why you're
interested in reading (free text, sent to the live AI for a genuine reply, or the same fallback
answers used elsewhere if no API key is configured). Answering both submits a real
`BookAccessRequest` row via AJAX (`Controllers/ProductsController.cs`,
`wwwroot/js/books-gate.js`). The category — and every book's product page — stays hidden, excluded
from search, the Shop grid, and the homepage's featured list, until an Admin reviews and approves
the request from **Admin → Book Requests**. Approval is per-customer, not global. Login is gated at
this step specifically (not site-wide), matching how Cart/Checkout already work.

## Wishlist

Signed-in users can tap the heart icon on any product card or product detail page to save it —
backed by a real `WishlistItems` table (`Infrastructure/Persistence/Configurations/
WishlistItemConfiguration.cs`), toggled via `POST /Wishlist/Toggle` (AJAX, no page reload) and
viewable at `/Wishlist`. The navbar heart badge updates live.

## AI Shopping Assistant ("Heritage Guide")

A real, server-side chat assistant (bottom-right widget on every page), backed by
`HeritageMarket.Infrastructure.AI.AiAssistantService`:

- **Server-side only** — the Anthropic API key never reaches the browser. The client calls
  `POST /Assistant/Ask` on our own server, which calls Anthropic's Messages API.
- **Grounded in the real catalog** — each request builds a compact system prompt from the live
  product list (`IProductService.GetCatalogAsync`), so answers reference actual products/prices.
- **Graceful fallback** — with no API key configured (the default), or if the live call fails,
  a rule-based fallback answers common questions (shipping, returns, materials, sizing) instead of
  erroring out.
- **Rate-limited** — `POST /Assistant/Ask` is capped at 12 requests/minute per user (or per IP for
  guests) via ASP.NET Core's built-in rate limiter, and each message/history is capped server-side
  (800 chars, last 6 turns) to keep token usage predictable.

To enable live answers, set an Anthropic API key (never commit a real key):

```powershell
dotnet user-secrets init --project src/HeritageMarket.Web
dotnet user-secrets set "AiAssistant:ApiKey" "sk-ant-..." --project src/HeritageMarket.Web
```

Model/limits are configurable under `AiAssistant` in `appsettings.json`
(`Model`, `MaxTokens`, `MaxHistoryMessages`, `MaxMessageLength`).

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
