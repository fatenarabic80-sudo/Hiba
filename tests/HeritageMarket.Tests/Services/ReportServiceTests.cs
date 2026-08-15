using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Implementations;
using HeritageMarket.Domain.Entities;
using HeritageMarket.Domain.Enums;
using HeritageMarket.Domain.Interfaces;
using HeritageMarket.Tests.TestHelpers;
using Moq;
using Xunit;

namespace HeritageMarket.Tests.Services;

public class ReportServiceTests
{
    [Fact]
    public async Task GetSalesReportAsync_ExcludesCancelledOrders_AndComputesTotals()
    {
        using var uow = TestUnitOfWorkFactory.Create();
        var country = new Country { Name = "Lebanon", Code = "LB" };
        var category = new Category { Name = "Home & Decoration" };
        await uow.Countries.AddAsync(country);
        await uow.Categories.AddAsync(category);
        await uow.SaveChangesAsync();

        var product = new Product { Name = "Cedar Box", Price = 45m, StockQuantity = 20, SKU = "LB-001", IsActive = true, CategoryId = category.Id, CountryId = country.Id };
        await uow.Products.AddAsync(product);
        await uow.SaveChangesAsync();

        var now = DateTime.UtcNow;

        await uow.Orders.AddAsync(new Order
        {
            ApplicationUserId = "u1",
            OrderDate = now.AddDays(-1),
            Status = OrderStatus.Delivered,
            TotalAmount = 90m,
            ShippingAddress = "Addr",
            ShippingCity = "City",
            Items = { new OrderItem { ProductId = product.Id, Product = product, Quantity = 2, UnitPrice = 45m } }
        });

        await uow.Orders.AddAsync(new Order
        {
            ApplicationUserId = "u2",
            OrderDate = now.AddDays(-1),
            Status = OrderStatus.Cancelled,
            TotalAmount = 999m,
            ShippingAddress = "Addr",
            ShippingCity = "City",
            Items = { new OrderItem { ProductId = product.Id, Product = product, Quantity = 100, UnitPrice = 45m } }
        });

        await uow.SaveChangesAsync();

        var directory = new Mock<IUserDirectoryService>();
        var reportService = new ReportService(uow, directory.Object);

        var report = await reportService.GetSalesReportAsync(new SalesReportRequest
        {
            FromDate = now.AddDays(-7),
            ToDate = now
        });

        Assert.Equal(90m, report.TotalRevenue);
        Assert.Equal(1, report.TotalOrders);
        Assert.Single(report.TopProducts);
        Assert.Equal(2, report.TopProducts[0].QuantitySold);
    }
}
