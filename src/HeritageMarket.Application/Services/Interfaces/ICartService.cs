using HeritageMarket.Application.DTOs;

namespace HeritageMarket.Application.Services.Interfaces;

public interface ICartService
{
    Task<CartDto> GetCartAsync(string applicationUserId);
    Task AddToCartAsync(string applicationUserId, int productId, int quantity, string? selectedSize = null);
    Task UpdateQuantityAsync(string applicationUserId, int cartItemId, int quantity);
    Task RemoveItemAsync(string applicationUserId, int cartItemId);
    Task ClearCartAsync(string applicationUserId);
}
