using HeritageMarket.Application.DTOs;

namespace HeritageMarket.Web.ViewModels;

/// <summary>
/// A clean, filter-free showcase of everything a single country offers — every product across every
/// category, grouped by category. Distinct from the Shop page, which keeps its full filter sidebar.
/// </summary>
public class CountryShowcaseViewModel
{
    public CountryDto Country { get; set; } = null!;
    public IReadOnlyList<ProductListItemDto> Products { get; set; } = Array.Empty<ProductListItemDto>();
}
