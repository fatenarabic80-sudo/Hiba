namespace HeritageMarket.Application.DTOs;

public class ProductListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public int StockQuantity { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public double AverageRating { get; set; }
}

public class ProductDetailDto : ProductListItemDto
{
    public string? Description { get; set; }
    public string SKU { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int CountryId { get; set; }
    public bool IsActive { get; set; }
    public List<ReviewDto> Reviews { get; set; } = new();
}

public class ProductEditDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public string SKU { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int CategoryId { get; set; }
    public int CountryId { get; set; }
}

public class ProductFilter
{
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public int? CountryId { get; set; }

    /// <summary>When set (and CategoryId isn't specifically this category), excludes it from results —
    /// used to keep the gated Heritage Books category out of general browsing/search/featured lists.</summary>
    public int? ExcludeCategoryId { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 9;
}
