using HeritageMarket.Application.Common;
using HeritageMarket.Application.Services.Implementations;
using HeritageMarket.Domain.Entities;
using HeritageMarket.Infrastructure.Persistence;
using HeritageMarket.Tests.TestHelpers;
using Xunit;

namespace HeritageMarket.Tests.Services;

public class CartServiceTests
{
    private const string UserId = "user-1";

    private static async Task<(UnitOfWork uow, int productId)> SeedStoreAsync(int stock = 10)
    {
        var uow = TestUnitOfWorkFactory.Create();
        var country = new Country { Name = "Lebanon", Code = "LB" };
        var category = new Category { Name = "Home & Decoration" };
        await uow.Countries.AddAsync(country);
        await uow.Categories.AddAsync(category);
        await uow.SaveChangesAsync();

        var product = new Product { Name = "Cedar Box", Price = 45m, StockQuantity = stock, SKU = "LB-001", IsActive = true, CategoryId = category.Id, CountryId = country.Id };
        await uow.Products.AddAsync(product);
        await uow.SaveChangesAsync();

        return (uow, product.Id);
    }

    [Fact]
    public async Task AddToCartAsync_CreatesCartAndAddsItem_WhenNoneExists()
    {
        var (uow, productId) = await SeedStoreAsync();
        using var _ = uow;
        var service = new CartService(uow);

        await service.AddToCartAsync(UserId, productId, quantity: 2);

        var cart = await service.GetCartAsync(UserId);
        Assert.Single(cart.Items);
        Assert.Equal(2, cart.Items[0].Quantity);
        Assert.Equal(90m, cart.Total);
    }

    [Fact]
    public async Task AddToCartAsync_IncrementsQuantity_WhenItemAlreadyInCart()
    {
        var (uow, productId) = await SeedStoreAsync();
        using var _ = uow;
        var service = new CartService(uow);

        await service.AddToCartAsync(UserId, productId, 2);
        await service.AddToCartAsync(UserId, productId, 3);

        var cart = await service.GetCartAsync(UserId);
        Assert.Single(cart.Items);
        Assert.Equal(5, cart.Items[0].Quantity);
    }

    [Fact]
    public async Task AddToCartAsync_ThrowsInsufficientStock_WhenQuantityExceedsStock()
    {
        var (uow, productId) = await SeedStoreAsync(stock: 3);
        using var _ = uow;
        var service = new CartService(uow);

        await Assert.ThrowsAsync<InsufficientStockException>(() => service.AddToCartAsync(UserId, productId, 5));
    }

    [Fact]
    public async Task RemoveItemAsync_RemovesItemFromCart()
    {
        var (uow, productId) = await SeedStoreAsync();
        using var _ = uow;
        var service = new CartService(uow);
        await service.AddToCartAsync(UserId, productId, 1);
        var cart = await service.GetCartAsync(UserId);
        var itemId = cart.Items[0].Id;

        await service.RemoveItemAsync(UserId, itemId);

        var updated = await service.GetCartAsync(UserId);
        Assert.Empty(updated.Items);
    }
}
