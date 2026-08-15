using HeritageMarket.Application.DTOs;

namespace HeritageMarket.Web.ViewModels;

public class HomeIndexViewModel
{
    public IReadOnlyList<ProductListItemDto> FeaturedProducts { get; set; } = Array.Empty<ProductListItemDto>();
    public IReadOnlyList<CategoryDto> Categories { get; set; } = Array.Empty<CategoryDto>();
    public IReadOnlyList<CountryDto> Countries { get; set; } = Array.Empty<CountryDto>();
}
