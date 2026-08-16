using HeritageMarket.Application.DTOs;

namespace HeritageMarket.Application.Services.Interfaces;

public interface IWishlistService
{
    Task<IReadOnlyList<ProductListItemDto>> GetWishlistAsync(string applicationUserId);
    Task<HashSet<int>> GetWishlistedProductIdsAsync(string applicationUserId);

    /// <summary>Adds the product if not already wishlisted, otherwise removes it. Returns the new state (true = wishlisted).</summary>
    Task<bool> ToggleAsync(string applicationUserId, int productId);
}
