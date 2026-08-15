using HeritageMarket.Application.DTOs;

namespace HeritageMarket.Application.Services.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CategoryDto dto);
    Task UpdateAsync(CategoryDto dto);
    Task DeleteAsync(int id);
}
