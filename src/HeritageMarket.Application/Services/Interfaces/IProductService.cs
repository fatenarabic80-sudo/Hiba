using HeritageMarket.Application.Common;
using HeritageMarket.Application.DTOs;

namespace HeritageMarket.Application.Services.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductListItemDto>> GetCatalogAsync(ProductFilter filter);
    Task<PagedResult<ProductListItemDto>> GetForAdminAsync(int pageNumber, int pageSize, string? search);
    Task<ProductDetailDto?> GetDetailAsync(int id);
    Task<ProductEditDto?> GetForEditAsync(int id);
    Task<IReadOnlyList<ProductListItemDto>> GetFeaturedAsync(int count);
    Task<int> CreateAsync(ProductEditDto dto);
    Task UpdateAsync(ProductEditDto dto);
    Task DeleteAsync(int id);
}
