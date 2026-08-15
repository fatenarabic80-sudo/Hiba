using HeritageMarket.Application.Common;
using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Domain.Entities;
using HeritageMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeritageMarket.Application.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync()
    {
        return await _unitOfWork.Categories.Query().AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IconUrl = c.IconUrl,
                ProductCount = c.Products.Count
            })
            .ToListAsync();
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var c = await _unitOfWork.Categories.GetByIdAsync(id);
        if (c is null) return null;

        return new CategoryDto { Id = c.Id, Name = c.Name, Description = c.Description, IconUrl = c.IconUrl };
    }

    public async Task<int> CreateAsync(CategoryDto dto)
    {
        var category = new Category { Name = dto.Name, Description = dto.Description, IconUrl = dto.IconUrl };
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return category.Id;
    }

    public async Task UpdateAsync(CategoryDto dto)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(dto.Id)
            ?? throw new NotFoundException($"Category {dto.Id} not found.");

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.IconUrl = dto.IconUrl;

        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id)
            ?? throw new NotFoundException($"Category {id} not found.");

        _unitOfWork.Categories.Remove(category);
        await _unitOfWork.SaveChangesAsync();
    }
}
