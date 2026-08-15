# Diagrams — Heritage Marketplace

These render natively on GitHub/GitLab/Azure DevOps (Mermaid support) and in most Markdown
previewers, including VS Code with the Markdown Preview Mermaid Support extension.

## 1. Entity-Relationship Diagram

```mermaid
erDiagram
    APPLICATION_USER ||--o| CART : owns
    APPLICATION_USER ||--o{ ORDER : places
    APPLICATION_USER ||--o{ REVIEW : writes

    COUNTRY ||--o{ PRODUCT : "represents heritage of"
    CATEGORY ||--o{ PRODUCT : classifies

    PRODUCT ||--o{ CART_ITEM : "added as"
    PRODUCT ||--o{ ORDER_ITEM : "purchased as"
    PRODUCT ||--o{ REVIEW : receives
    PRODUCT |o--o{ NOTIFICATION : "flags low stock on"

    CART ||--o{ CART_ITEM : contains
    ORDER ||--o{ ORDER_ITEM : contains

    APPLICATION_USER {
        string Id PK
        string Email
        string FullName
        string Address
        string ProfileImageUrl
        datetime CreatedAt
    }
    COUNTRY {
        int Id PK
        string Name
        string Code
        string FlagImageUrl
        string Description
    }
    CATEGORY {
        int Id PK
        string Name
        string Description
        string IconUrl
    }
    PRODUCT {
        int Id PK
        string Name
        string Description
        decimal Price
        int StockQuantity
        string ImageUrl
        string SKU
        bool IsActive
        datetime CreatedAt
        int CategoryId FK
        int CountryId FK
    }
    CART {
        int Id PK
        string ApplicationUserId FK
    }
    CART_ITEM {
        int Id PK
        int CartId FK
        int ProductId FK
        int Quantity
    }
    ORDER {
        int Id PK
        string ApplicationUserId FK
        datetime OrderDate
        string Status
        decimal TotalAmount
        string ShippingAddress
        string ShippingCity
    }
    ORDER_ITEM {
        int Id PK
        int OrderId FK
        int ProductId FK
        int Quantity
        decimal UnitPrice
    }
    REVIEW {
        int Id PK
        int ProductId FK
        string ApplicationUserId FK
        int Rating
        string Comment
        datetime CreatedAt
    }
    NOTIFICATION {
        int Id PK
        string Message
        datetime CreatedAt
        bool IsRead
        int ProductId FK
    }
```

## 2. System Architecture Diagram

```mermaid
flowchart TB
    subgraph Client["Clients"]
        Browser["Browser (Razor Views + Bootstrap)"]
        ExternalApp["External Client (mobile/other service)"]
    end

    subgraph Web["HeritageMarket.Web — Presentation Layer"]
        MVC["MVC Controllers + Views<br/>(Storefront, Account, Admin area)"]
        Api["REST API Controllers<br/>(/api/*, JWT-secured)"]
        Swagger["Swagger / OpenAPI docs"]
    end

    subgraph App["HeritageMarket.Application — Business Logic Layer"]
        Services["Services<br/>Product / Cart / Order / Review /<br/>Notification / Report / Token"]
        DTOs["DTOs"]
    end

    subgraph Infra["HeritageMarket.Infrastructure — Data Access Layer"]
        UoW["UnitOfWork + Repositories"]
        EFCore["EF Core ApplicationDbContext"]
        Identity["ASP.NET Core Identity"]
        BgService["LowStockNotificationService<br/>(BackgroundService)"]
        Jwt["JWT TokenService"]
    end

    subgraph Domain["HeritageMarket.Domain — Entities & Contracts"]
        Entities["Entities & Enums"]
        Interfaces["IRepository / IUnitOfWork<br/>interfaces"]
    end

    DB[("SQL Server")]

    Browser --> MVC
    ExternalApp --> Api
    Api -.-> Swagger
    MVC --> Services
    Api --> Services
    Services --> DTOs
    Services --> Interfaces
    UoW -.implements.-> Interfaces
    Services --> UoW
    UoW --> EFCore
    EFCore --> DB
    Identity --> EFCore
    Jwt --> Api
    BgService --> UoW
    Entities --- Interfaces

    style Domain fill:#f4e7d7,stroke:#8a4b2f
    style App fill:#efe0c8,stroke:#8a4b2f
    style Infra fill:#e8d3b0,stroke:#8a4b2f
    style Web fill:#faf3e9,stroke:#8a4b2f
```

## 3. Use Case Diagram

Mermaid has no native UML use-case shape, so actors and use cases are modeled as a flowchart —
actor nodes on the left, use cases grouped by area on the right.

```mermaid
flowchart LR
    Guest(("Guest"))
    Customer(("Customer"))
    Admin(("Admin"))

    subgraph Storefront["Storefront use cases"]
        UC1["Browse / search catalog"]
        UC2["View product details & reviews"]
        UC3["Register / Login"]
        UC4["Manage cart"]
        UC5["Checkout"]
        UC6["View order history"]
        UC7["Write a product review"]
        UC8["Manage profile"]
    end

    subgraph AdminUC["Admin use cases"]
        UC9["Manage products (CRUD + image)"]
        UC10["Manage categories"]
        UC11["Manage countries"]
        UC12["Update order status"]
        UC13["Manage users & roles"]
        UC14["View dashboard & sales reports"]
        UC15["Receive low-stock notifications"]
    end

    subgraph ApiUC["External API client use cases"]
        UC16["Authenticate (JWT)"]
        UC17["Browse catalog via API"]
        UC18["Manage cart via API"]
        UC19["Place order via API"]
    end

    Guest --> UC1
    Guest --> UC2
    Guest --> UC3

    Customer --> UC1
    Customer --> UC2
    Customer --> UC4
    Customer --> UC5
    Customer --> UC6
    Customer --> UC7
    Customer --> UC8

    Admin --> UC9
    Admin --> UC10
    Admin --> UC11
    Admin --> UC12
    Admin --> UC13
    Admin --> UC14
    Admin --> UC15

    Customer -.->|"via external app"| UC16
    UC16 --> UC17
    UC16 --> UC18
    UC16 --> UC19
```
