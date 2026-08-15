using HeritageMarket.Application.Common;
using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Domain.Entities;
using HeritageMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeritageMarket.Application.Services.Implementations;

public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;

    public CartService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private async Task<Cart> GetOrCreateCartAsync(string applicationUserId)
    {
        var cart = await _unitOfWork.Carts.GetByUserIdAsync(applicationUserId);
        if (cart is not null) return cart;

        cart = new Cart { ApplicationUserId = applicationUserId };
        await _unitOfWork.Carts.AddAsync(cart);
        await _unitOfWork.SaveChangesAsync();
        return cart;
    }

    public async Task<CartDto> GetCartAsync(string applicationUserId)
    {
        var cart = await _unitOfWork.Carts.Query().AsNoTracking()
            .Where(c => c.ApplicationUserId == applicationUserId)
            .Select(c => new CartDto
            {
                Id = c.Id,
                Items = c.Items.Select(i => new CartItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product.Name,
                    ProductImageUrl = i.Product.ImageUrl,
                    UnitPrice = i.Product.Price,
                    Quantity = i.Quantity,
                    AvailableStock = i.Product.StockQuantity
                }).ToList()
            })
            .FirstOrDefaultAsync();

        return cart ?? new CartDto();
    }

    public async Task AddToCartAsync(string applicationUserId, int productId, int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

        var product = await _unitOfWork.Products.GetByIdAsync(productId)
            ?? throw new NotFoundException($"Product {productId} not found.");

        var cart = await GetOrCreateCartAsync(applicationUserId);
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        var requestedTotal = (existingItem?.Quantity ?? 0) + quantity;

        if (requestedTotal > product.StockQuantity)
            throw new InsufficientStockException($"Only {product.StockQuantity} unit(s) of '{product.Name}' are available.");

        if (existingItem is not null)
        {
            existingItem.Quantity = requestedTotal;
        }
        else
        {
            cart.Items.Add(new CartItem { CartId = cart.Id, ProductId = productId, Quantity = quantity });
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateQuantityAsync(string applicationUserId, int cartItemId, int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

        var cart = await _unitOfWork.Carts.GetByUserIdAsync(applicationUserId)
            ?? throw new NotFoundException("Cart not found.");

        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId)
            ?? throw new NotFoundException($"Cart item {cartItemId} not found.");

        var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId)
            ?? throw new NotFoundException($"Product {item.ProductId} not found.");

        if (quantity > product.StockQuantity)
            throw new InsufficientStockException($"Only {product.StockQuantity} unit(s) of '{product.Name}' are available.");

        item.Quantity = quantity;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RemoveItemAsync(string applicationUserId, int cartItemId)
    {
        var cart = await _unitOfWork.Carts.GetByUserIdAsync(applicationUserId)
            ?? throw new NotFoundException("Cart not found.");

        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId);
        if (item is not null)
        {
            cart.Items.Remove(item);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task ClearCartAsync(string applicationUserId)
    {
        var cart = await _unitOfWork.Carts.GetByUserIdAsync(applicationUserId);
        if (cart is null || cart.Items.Count == 0) return;

        cart.Items.Clear();
        await _unitOfWork.SaveChangesAsync();
    }
}
