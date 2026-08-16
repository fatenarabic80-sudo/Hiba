using HeritageMarket.Application.Services.Implementations;
using HeritageMarket.Domain.Entities;
using HeritageMarket.Tests.TestHelpers;
using Xunit;

namespace HeritageMarket.Tests.Services;

public class WishlistServiceTests
{
    private const string UserId = "user-1";

    private static async Task<(HeritageMarket.Infrastructure.Persistence.UnitOfWork uow, int productId)> SeedStoreAsync()
    {
        var uow = TestUnitOfWorkFactory.Create();
        var country = new Country { Name = "Lebanon", Code = "LB" };
        var category = new Category { Name = "Home & Decoration" };
        await uow.Countries.AddAsync(country);
        await uow.Categories.AddAsync(category);
        await uow.SaveChangesAsync();

        var product = new Product { Name = "Cedar Box", Price = 45m, StockQuantity = 10, SKU = "LB-001", IsActive = true, CategoryId = category.Id, CountryId = country.Id };
        await uow.Products.AddAsync(product);
        await uow.SaveChangesAsync();

        return (uow, product.Id);
    }

    [Fact]
    public async Task ToggleAsync_AddsThenRemoves()
    {
        var (uow, productId) = await SeedStoreAsync();
        using var _ = uow;
        var service = new WishlistService(uow);

        var afterFirstToggle = await service.ToggleAsync(UserId, productId);
        Assert.True(afterFirstToggle);

        var ids = await service.GetWishlistedProductIdsAsync(UserId);
        Assert.Contains(productId, ids);

        var afterSecondToggle = await service.ToggleAsync(UserId, productId);
        Assert.False(afterSecondToggle);

        var idsAfterRemoval = await service.GetWishlistedProductIdsAsync(UserId);
        Assert.DoesNotContain(productId, idsAfterRemoval);
    }

    [Fact]
    public async Task GetWishlistAsync_ReturnsWishlistedProductDetails()
    {
        var (uow, productId) = await SeedStoreAsync();
        using var _ = uow;
        var service = new WishlistService(uow);

        await service.ToggleAsync(UserId, productId);

        var wishlist = await service.GetWishlistAsync(UserId);

        Assert.Single(wishlist);
        Assert.Equal("Cedar Box", wishlist[0].Name);
    }

    [Fact]
    public async Task GetWishlistedProductIdsAsync_IsEmptyForNewUser()
    {
        var (uow, _) = await SeedStoreAsync();
        using var _uow = uow;
        var service = new WishlistService(uow);

        var ids = await service.GetWishlistedProductIdsAsync(UserId);

        Assert.Empty(ids);
    }
}
