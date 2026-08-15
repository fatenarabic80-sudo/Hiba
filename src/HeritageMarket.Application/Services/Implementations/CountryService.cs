using HeritageMarket.Application.Common;
using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Domain.Entities;
using HeritageMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeritageMarket.Application.Services.Implementations;

public class CountryService : ICountryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CountryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<CountryDto>> GetAllAsync()
    {
        return await _unitOfWork.Countries.Query().AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CountryDto
            {
                Id = c.Id,
                Name = c.Name,
                Code = c.Code,
                FlagImageUrl = c.FlagImageUrl,
                Description = c.Description,
                ProductCount = c.Products.Count
            })
            .ToListAsync();
    }

    public async Task<CountryDto?> GetByIdAsync(int id)
    {
        var c = await _unitOfWork.Countries.GetByIdAsync(id);
        if (c is null) return null;

        return new CountryDto { Id = c.Id, Name = c.Name, Code = c.Code, FlagImageUrl = c.FlagImageUrl, Description = c.Description };
    }

    public async Task<int> CreateAsync(CountryDto dto)
    {
        var country = new Country { Name = dto.Name, Code = dto.Code, FlagImageUrl = dto.FlagImageUrl, Description = dto.Description };
        await _unitOfWork.Countries.AddAsync(country);
        await _unitOfWork.SaveChangesAsync();
        return country.Id;
    }

    public async Task UpdateAsync(CountryDto dto)
    {
        var country = await _unitOfWork.Countries.GetByIdAsync(dto.Id)
            ?? throw new NotFoundException($"Country {dto.Id} not found.");

        country.Name = dto.Name;
        country.Code = dto.Code;
        country.FlagImageUrl = dto.FlagImageUrl;
        country.Description = dto.Description;

        _unitOfWork.Countries.Update(country);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var country = await _unitOfWork.Countries.GetByIdAsync(id)
            ?? throw new NotFoundException($"Country {id} not found.");

        _unitOfWork.Countries.Remove(country);
        await _unitOfWork.SaveChangesAsync();
    }
}
