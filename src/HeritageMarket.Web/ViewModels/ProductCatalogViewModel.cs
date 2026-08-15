using HeritageMarket.Application.Common;
using HeritageMarket.Application.DTOs;

namespace HeritageMarket.Web.ViewModels;

public class ProductCatalogViewModel
{
    public PagedResult<ProductListItemDto> Products { get; set; } = new();
    public IReadOnlyList<CategoryDto> Categories { get; set; } = Array.Empty<CategoryDto>();
    public IReadOnlyList<CountryDto> Countries { get; set; } = Array.Empty<CountryDto>();
    public ProductFilter Filter { get; set; } = new();
}

public class ProductDetailsViewModel
{
    public ProductDetailDto Product { get; set; } = null!;
    public ReviewFormViewModel ReviewForm { get; set; } = new();
    public bool CanReview { get; set; }
}
