# Presentation Outline — Heritage Marketplace

Slide-by-slide content, ready to drop into PowerPoint/Google Slides. The live demo itself needs to
be run by you from the actual application (see "Demo Script" at the end) — that part can't be
prepared in advance as a document.

---

**Slide 1 — Title**
Heritage Marketplace
An Online Marketplace for Heritage-Inspired Products
COMP420 — Application Development

**Slide 2 — The Problem**
- Culturally-themed products (heritage decor, traditional wear, regional crafts) are scattered
  across niche sellers with no unified storefront
- No single place to browse products *by the culture/country they represent*

**Slide 3 — The Solution**
- A marketplace where every product is tagged by both **category** and **country of origin**
- Browse "Home & Decoration" *or* browse "everything from Morocco" — same catalog, two lenses
- Admin-managed catalog, customer accounts with cart/checkout/order history/reviews

**Slide 4 — Tech Stack**
- ASP.NET Core 8 MVC, C#
- Entity Framework Core 8 (Code First), SQL Server
- ASP.NET Core Identity (cookie auth for the site, JWT for the REST API)
- Serilog, Swagger, Bootstrap 5, Chart.js
- xUnit + Moq + EF Core InMemory for testing

**Slide 5 — Architecture**
- N-Layer: Domain → Application → Infrastructure/Web
- Repository + Unit of Work pattern
- Dependency Injection throughout
*(insert the System Architecture Diagram from docs/Diagrams.md)*

**Slide 6 — Database Design**
- 9 core tables + ASP.NET Identity tables
- Key relationships: Country/Category → Product; Cart/Order → line items; Product → Reviews
*(insert the ER Diagram from docs/Diagrams.md)*

**Slide 7 — Core Features**
- Role-based auth (Admin / Customer)
- Full shopping flow: browse → cart → checkout → order history
- Admin panel: product/category/country CRUD with image upload, order management, user management
- Sales dashboard + CSV-exportable reports with charts
- Automated low-stock notifications (background service)

**Slide 8 — REST API**
- JWT-secured API alongside the cookie-based MVC site
- Swagger-documented: catalog browsing, cart, and order endpoints
- Built for a real external client (mobile app, other service), not just the browser

**Slide 9 — Security**
- Anti-forgery tokens, hashed + lockout-protected passwords
- Upload validation (type/size/generated filenames)
- HTTPS/HSTS, parameterized EF Core queries, Razor output encoding

**Slide 10 — Testing & Quality**
- 11 xUnit tests covering cart/order/product/report business logic
- Structured logging (Serilog) + centralized exception handling middleware

**Slide 11 — Challenges**
*(fill in with your own experience once you've run/extended the project — e.g. getting EF Core to
translate DTO projections to SQL instead of failing at runtime, wiring two auth schemes — cookie +
JWT — side by side, etc.)*

**Slide 12 — Demo**
Live walkthrough — see script below.

**Slide 13 — Questions**

---

## Demo Script (to run live, not a slide)

1. **Customer flow**: open the home page → filter the catalog by country → open a product → add
   it to the cart → checkout → show the order in "My Orders."
2. **Admin flow**: log in as `admin@heritagemarket.local` → Dashboard (point out the KPIs and any
   low-stock notifications) → create/edit a product with an image upload → update an order's
   status → open the Reports page and show the chart + CSV export.
3. **API**: open `/swagger`, show the endpoint list, call `POST /api/auth/login`, then use the
   returned token to call an authenticated endpoint (e.g. `GET /api/orders`) directly from the
   Swagger UI.
