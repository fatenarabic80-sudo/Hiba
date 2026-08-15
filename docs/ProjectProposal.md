# Project Proposal — Heritage Marketplace

## Project Title

**Heritage Marketplace** — An Online Marketplace for Heritage-Inspired Products

## Project Overview

Heritage Marketplace is a web-based e-commerce platform for products inspired by the history,
culture, and traditions of countries around the world. Instead of organizing a store purely by
product type, every item is tagged with both a **category** (Home & Decoration, Accessories, Phone
Covers, Wear & Traditional Clothing, Heritage Books) and a **country of origin**, letting shoppers
browse either by what they want or by which culture they're interested in.

**Problem it solves**: small, culturally-themed products (hand-crafted decor, traditional textiles,
heritage literature) are usually scattered across many niche or regional sellers with no unified,
searchable storefront. Heritage Marketplace gives them one catalog, one checkout flow, and one
place for customers to discover products tied to a specific country's heritage.

**Intended users**:
- **Customers** — browse the catalog by category/country, search, read and leave reviews, manage a
  cart, check out, and track their order history.
- **Admin** — manages the entire catalog (products, categories, countries), fulfills and updates
  orders, manages user accounts/roles, and reviews sales reports and low-stock alerts.

## Scope of the Project

**In scope:**
- Product catalog with category + country filtering, search, and paging
- Shopping cart and checkout with stock validation
- Order history and admin order-status management
- Product reviews and ratings
- Admin CRUD for products (with image upload), categories, and countries
- User registration/login, profile management (with profile picture upload), password reset
- Role-based access control (Admin / Customer)
- Admin sales dashboard and CSV-exportable sales reports
- A REST API (JWT-secured) exposing the catalog and order/cart operations for external clients
- Automated low-stock notifications via a background service

**Out of scope (explicitly not included):**
- Multi-vendor / seller accounts — the catalog is centrally managed by the Admin role, not
  independent third-party sellers
- Real payment processing (no live payment gateway integration; checkout is a demo flow)
- Real-time features (chat, live order tracking via SignalR)
- Shipping-carrier integration or live shipping cost calculation
- Multi-language / multi-currency support

## Expected Database Structure

Main entities and their relationships:

- **ApplicationUser** (Identity) — registered accounts (Admin or Customer role)
- **Country** (1) → (\*) **Product** — the heritage/culture a product represents
- **Category** (1) → (\*) **Product** — the kind of product (decor, accessories, phone covers,
  wear, books)
- **Product** — the sellable item; belongs to one Category and one Country
- **Cart** (1) → (\*) **CartItem**, one Cart per ApplicationUser — the shopping cart in progress
- **Order** (1) → (\*) **OrderItem**, many Orders per ApplicationUser — a completed purchase, with
  a status (Pending → Processing → Shipped → Delivered, or Cancelled)
- **Review** — a Product review/rating left by an ApplicationUser
- **Notification** — system-generated alerts (e.g. low stock on a Product), surfaced to the Admin

## Team Members

*(Excluded per course instructions for this submission.)*

## Initial System Pages & Functionalities

Roughly 30+ pages/screens across these areas:

- **Storefront**: Home, Product Catalog (filter/search/paging), Product Details (+ reviews), Cart,
  Checkout, Order Confirmation, My Orders, Order Details
- **Account**: Login, Register, Forgot Password, Reset Password, My Profile (settings + password
  change + profile picture)
- **Admin**: Dashboard (KPIs + low-stock alerts), Products CRUD (with image upload), Categories
  CRUD, Countries CRUD, Orders (list + status update), Users (roles + lockout), Reports (date-range
  sales report, top products, CSV export)
- **API**: Swagger-documented REST endpoints for authentication, catalog browsing, cart, and orders
