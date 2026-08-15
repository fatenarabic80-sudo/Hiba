using HeritageMarket.Application.Common;
using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Implementations;
using HeritageMarket.Domain.Entities;
using HeritageMarket.Domain.Interfaces;
using HeritageMarket.Infrastructure.Persistence;
using HeritageMarket.Tests.TestHelpers;
using Moq;
using Xunit;

namespace HeritageMarket.Tests.Services;

public class OrderServiceTests
{
    private const string UserId = "user-1";

    private static async Task<(UnitOfWork uow, int productId, Mock<IUserDirectoryService> directory)> SeedStoreAsync(int stock = 10)
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

        var directory = new Mock<IUserDirectoryService>();
        directory.Setup(d => d.GetDisplayNameAsync(UserId)).ReturnsAsync("Test Customer");

        return (uow, product.Id, directory);
    }

    [Fact]
    public async Task PlaceOrderAsync_ThrowsInvalidOperation_WhenCartIsEmpty()
    {
        var (uow, _, directory) = await SeedStoreAsync();
        using var _uow = uow;
        var orderService = new OrderService(uow, directory.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            orderService.PlaceOrderAsync(new PlaceOrderRequest { ApplicationUserId = UserId, ShippingAddress = "Addr", ShippingCity = "City" }));
    }

    [Fact]
    public async Task PlaceOrderAsync_DecrementsStock_ComputesTotal_AndClearsCart()
    {
        var (uow, productId, directory) = await SeedStoreAsync(stock: 10);
        using var _uow = uow;
        var cartService = new CartService(uow);
        await cartService.AddToCartAsync(UserId, productId, 3);

        var orderService = new OrderService(uow, directory.Object);
        var order = await orderService.PlaceOrderAsync(new PlaceOrderRequest { ApplicationUserId = UserId, ShippingAddress = "123 Cedar St", ShippingCity = "Beirut" });

        Assert.Equal(135m, order.TotalAmount);

        var product = await uow.Products.GetByIdAsync(productId);
        Assert.Equal(7, product!.StockQuantity);

        var cart = await cartService.GetCartAsync(UserId);
        Assert.Empty(cart.Items);
    }

    [Fact]
    public async Task PlaceOrderAsync_ThrowsInsufficientStock_WhenCartExceedsCurrentStock()
    {
        var (uow, productId, directory) = await SeedStoreAsync(stock: 5);
        using var _uow = uow;
        var cartService = new CartService(uow);
        await cartService.AddToCartAsync(UserId, productId, 5);

        // Stock drops after the item was added to the cart (e.g. another order consumed it).
        var product = await uow.Products.GetByIdAsync(productId);
        product!.StockQuantity = 1;
        await uow.SaveChangesAsync();

        var orderService = new OrderService(uow, directory.Object);

        await Assert.ThrowsAsync<InsufficientStockException>(() =>
            orderService.PlaceOrderAsync(new PlaceOrderRequest { ApplicationUserId = UserId, ShippingAddress = "Addr", ShippingCity = "City" }));
    }
}
