using System.Linq.Expressions;
using HeritageMarket.Application.Common;
using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Domain.Entities;
using HeritageMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeritageMarket.Application.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private static IQueryable<Product> ApplyFilter(IQueryable<Product> query, string? searchTerm, int? categoryId, int? countryId, int? excludeCategoryId = null)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(p => p.Name.Contains(term) || p.SKU.Contains(term));
        }

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId);

        if (countryId.HasValue)
            query = query.Where(p => p.CountryId == countryId);

        if (excludeCategoryId.HasValue && categoryId != excludeCategoryId)
            query = query.Where(p => p.CategoryId != excludeCategoryId);

        return query;
    }

    // Kept as a translatable Expression<Func<>> (not a plain method) so EF Core can compose it
    // into the generated SQL (joins for Category/Country, aggregate for AverageRating) instead of
    // falling back to per-row client evaluation against un-included navigation properties.
    private static readonly Expression<Func<Product, ProductListItemDto>> ToListItemProjection = p => new ProductListItemDto
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price,
        ImageUrl = p.ImageUrl,
        StockQuantity = p.StockQuantity,
        CategoryName = p.Category.Name,
        CountryName = p.Country.Name,
        AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
        Gender = p.Gender
    };

    public async Task<PagedResult<ProductListItemDto>> GetCatalogAsync(ProductFilter filter)
    {
        var query = ApplyFilter(_unitOfWork.Products.Query().AsNoTracking().Where(p => p.IsActive),
            filter.SearchTerm, filter.CategoryId, filter.CountryId, filter.ExcludeCategoryId);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(ToListItemProjection)
            .ToListAsync();

        return new PagedResult<ProductListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<PagedResult<ProductListItemDto>> GetForAdminAsync(int pageNumber, int pageSize, string? search)
    {
        var query = ApplyFilter(_unitOfWork.Products.Query().AsNoTracking(), search, null, null);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(ToListItemProjection)
            .ToListAsync();

        return new PagedResult<ProductListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ProductDetailDto?> GetDetailAsync(int id)
    {
        return await _unitOfWork.Products.Query().AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductDetailDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                StockQuantity = p.StockQuantity,
                SKU = p.SKU,
                IsActive = p.IsActive,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                CountryId = p.CountryId,
                CountryName = p.Country.Name,
                AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
                Gender = p.Gender,
                Sizes = p.Sizes,
                Reviews = p.Reviews
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new ReviewDto
                    {
                        Id = r.Id,
                        ApplicationUserId = r.ApplicationUserId,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        CreatedAt = r.CreatedAt
                    }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ProductEditDto?> GetForEditAsync(int id)
    {
        var p = await _unitOfWork.Products.GetByIdAsync(id);
        if (p is null) return null;

        return new ProductEditDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            ImageUrl = p.ImageUrl,
            SKU = p.SKU,
            IsActive = p.IsActive,
            CategoryId = p.CategoryId,
            CountryId = p.CountryId,
            Gender = p.Gender,
            Sizes = p.Sizes
        };
    }

    public async Task<IReadOnlyList<ProductListItemDto>> GetFeaturedAsync(int count, int? excludeCategoryId = null)
    {
        var query = _unitOfWork.Products.Query().AsNoTracking().Where(p => p.IsActive);
        if (excludeCategoryId.HasValue)
            query = query.Where(p => p.CategoryId != excludeCategoryId);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .Select(ToListItemProjection)
            .ToListAsync();
    }

    public async Task<int> CreateAsync(ProductEditDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            ImageUrl = dto.ImageUrl,
            SKU = dto.SKU,
            IsActive = dto.IsActive,
            CategoryId = dto.CategoryId,
            CountryId = dto.CountryId,
            Gender = dto.Gender,
            Sizes = dto.Sizes,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return product.Id;
    }

    public async Task UpdateAsync(ProductEditDto dto)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(dto.Id)
            ?? throw new NotFoundException($"Product {dto.Id} not found.");

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.StockQuantity = dto.StockQuantity;
        if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            product.ImageUrl = dto.ImageUrl;
        product.SKU = dto.SKU;
        product.IsActive = dto.IsActive;
        product.CategoryId = dto.CategoryId;
        product.CountryId = dto.CountryId;
        product.Gender = dto.Gender;
        product.Sizes = dto.Sizes;

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id)
            ?? throw new NotFoundException($"Product {id} not found.");

        _unitOfWork.Products.Remove(product);
        await _unitOfWork.SaveChangesAsync();
    }
}
