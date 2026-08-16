using System.Linq.Expressions;
using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Domain.Entities;
using HeritageMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeritageMarket.Application.Services.Implementations;

public class WishlistService : IWishlistService
{
    private readonly IUnitOfWork _unitOfWork;

    private static readonly Expression<Func<Product, ProductListItemDto>> ToListItemProjection = p => new ProductListItemDto
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price,
        ImageUrl = p.ImageUrl,
        StockQuantity = p.StockQuantity,
        CategoryName = p.Category.Name,
        CountryName = p.Country.Name,
        AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0
    };

    public WishlistService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<ProductListItemDto>> GetWishlistAsync(string applicationUserId)
    {
        var productIds = _unitOfWork.WishlistItems.Query().AsNoTracking()
            .Where(w => w.ApplicationUserId == applicationUserId)
            .Select(w => w.ProductId);

        return await _unitOfWork.Products.Query().AsNoTracking()
            .Where(p => productIds.Contains(p.Id))
            .Select(ToListItemProjection)
            .ToListAsync();
    }

    public async Task<HashSet<int>> GetWishlistedProductIdsAsync(string applicationUserId)
    {
        var ids = await _unitOfWork.WishlistItems.Query().AsNoTracking()
            .Where(w => w.ApplicationUserId == applicationUserId)
            .Select(w => w.ProductId)
            .ToListAsync();

        return ids.ToHashSet();
    }

    public async Task<bool> ToggleAsync(string applicationUserId, int productId)
    {
        var existing = await _unitOfWork.WishlistItems.Query()
            .FirstOrDefaultAsync(w => w.ApplicationUserId == applicationUserId && w.ProductId == productId);

        if (existing is not null)
        {
            _unitOfWork.WishlistItems.Remove(existing);
            await _unitOfWork.SaveChangesAsync();
            return false;
        }

        await _unitOfWork.WishlistItems.AddAsync(new WishlistItem
        {
            ApplicationUserId = applicationUserId,
            ProductId = productId,
            CreatedAt = DateTime.UtcNow
        });
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
