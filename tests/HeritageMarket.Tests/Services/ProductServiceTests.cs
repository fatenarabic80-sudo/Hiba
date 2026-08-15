using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Implementations;
using HeritageMarket.Domain.Entities;
using HeritageMarket.Tests.TestHelpers;
using Xunit;

namespace HeritageMarket.Tests.Services;

public class ProductServiceTests
{
    private static async Task<HeritageMarket.Infrastructure.Persistence.UnitOfWork> SeedCatalogAsync()
    {
        var uow = TestUnitOfWorkFactory.Create();
        var country = new Country { Name = "Lebanon", Code = "LB" };
        var category = new Category { Name = "Home & Decoration" };
        await uow.Countries.AddAsync(country);
        await uow.Categories.AddAsync(category);
        await uow.SaveChangesAsync();

        await uow.Products.AddAsync(new Product { Name = "Cedar Box", Price = 45m, StockQuantity = 20, SKU = "LB-001", IsActive = true, CategoryId = category.Id, CountryId = country.Id, CreatedAt = DateTime.UtcNow });
        await uow.Products.AddAsync(new Product { Name = "Mosaic Tray", Price = 30m, StockQuantity = 0, SKU = "LB-002", IsActive = true, CategoryId = category.Id, CountryId = country.Id, CreatedAt = DateTime.UtcNow.AddMinutes(-1) });
        await uow.Products.AddAsync(new Product { Name = "Discontinued Item", Price = 10m, StockQuantity = 5, SKU = "LB-003", IsActive = false, CategoryId = category.Id, CountryId = country.Id, CreatedAt = DateTime.UtcNow.AddMinutes(-2) });
        await uow.SaveChangesAsync();

        return uow;
    }

    [Fact]
    public async Task GetCatalogAsync_ExcludesInactiveProducts()
    {
        using var uow = await SeedCatalogAsync();
        var service = new ProductService(uow);

        var result = await service.GetCatalogAsync(new ProductFilter { PageNumber = 1, PageSize = 10 });

        Assert.Equal(2, result.TotalCount);
        Assert.DoesNotContain(result.Items, p => p.Name == "Discontinued Item");
    }

    [Fact]
    public async Task GetCatalogAsync_FiltersBySearchTerm()
    {
        using var uow = await SeedCatalogAsync();
        var service = new ProductService(uow);

        var result = await service.GetCatalogAsync(new ProductFilter { SearchTerm = "Cedar", PageNumber = 1, PageSize = 10 });

        Assert.Single(result.Items);
        Assert.Equal("Cedar Box", result.Items[0].Name);
    }

    [Fact]
    public async Task GetCatalogAsync_RespectsPaging()
    {
        using var uow = await SeedCatalogAsync();
        var service = new ProductService(uow);

        var page1 = await service.GetCatalogAsync(new ProductFilter { PageNumber = 1, PageSize = 1 });
        var page2 = await service.GetCatalogAsync(new ProductFilter { PageNumber = 2, PageSize = 1 });

        Assert.Single(page1.Items);
        Assert.Single(page2.Items);
        Assert.NotEqual(page1.Items[0].Id, page2.Items[0].Id);
        Assert.Equal(2, page1.TotalPages);
    }
}
