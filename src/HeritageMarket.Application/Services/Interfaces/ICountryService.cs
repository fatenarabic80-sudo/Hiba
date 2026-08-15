using HeritageMarket.Application.DTOs;

namespace HeritageMarket.Application.Services.Interfaces;

public interface ICountryService
{
    Task<IReadOnlyList<CountryDto>> GetAllAsync();
    Task<CountryDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CountryDto dto);
    Task UpdateAsync(CountryDto dto);
    Task DeleteAsync(int id);
}
