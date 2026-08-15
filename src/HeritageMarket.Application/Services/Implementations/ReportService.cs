using System.Text;
using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Domain.Enums;
using HeritageMarket.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeritageMarket.Application.Services.Implementations;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserDirectoryService _userDirectory;

    public ReportService(IUnitOfWork unitOfWork, IUserDirectoryService userDirectory)
    {
        _unitOfWork = unitOfWork;
        _userDirectory = userDirectory;
    }

    private static IQueryable<Domain.Entities.Order> OrdersInRange(IQueryable<Domain.Entities.Order> query, DateTime from, DateTime to) =>
        query.Where(o => o.OrderDate >= from && o.OrderDate <= to && o.Status != OrderStatus.Cancelled);

    public async Task<SalesReportDto> GetSalesReportAsync(SalesReportRequest request)
    {
        var ordersQuery = OrdersInRange(_unitOfWork.Orders.Query().AsNoTracking(), request.FromDate, request.ToDate);

        var orders = await ordersQuery
            .Select(o => new { o.OrderDate, o.TotalAmount, Items = o.Items.Select(i => new { i.ProductId, i.Product.Name, i.Quantity, i.UnitPrice }) })
            .ToListAsync();

        var dailySales = orders
            .GroupBy(o => o.OrderDate.Date)
            .OrderBy(g => g.Key)
            .Select(g => new DailySalesDto
            {
                Date = g.Key,
                Revenue = g.Sum(o => o.TotalAmount),
                OrderCount = g.Count()
            })
            .ToList();

        var topProducts = orders
            .SelectMany(o => o.Items)
            .GroupBy(i => new { i.ProductId, i.Name })
            .Select(g => new TopProductDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                QuantitySold = g.Sum(i => i.Quantity),
                Revenue = g.Sum(i => i.Quantity * i.UnitPrice)
            })
            .OrderByDescending(p => p.Revenue)
            .Take(5)
            .ToList();

        return new SalesReportDto
        {
            TotalRevenue = orders.Sum(o => o.TotalAmount),
            TotalOrders = orders.Count,
            DailySales = dailySales,
            TopProducts = topProducts
        };
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
    {
        var ordersQuery = _unitOfWork.Orders.Query().AsNoTracking().Where(o => o.Status != OrderStatus.Cancelled);

        var totalRevenue = await ordersQuery.SumAsync(o => (decimal?)o.TotalAmount) ?? 0;
        var totalOrders = await _unitOfWork.Orders.Query().AsNoTracking().CountAsync();
        var totalProducts = await _unitOfWork.Products.Query().AsNoTracking().CountAsync();
        var totalCustomers = await _userDirectory.GetCustomerCountAsync();

        var recentOrders = await _unitOfWork.Orders.Query().AsNoTracking()
            .OrderByDescending(o => o.OrderDate)
            .Take(5)
            .Select(o => new OrderListItemDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                ItemCount = o.Items.Sum(i => i.Quantity)
            })
            .ToListAsync();

        var recentNotifications = await _unitOfWork.Notifications.Query().AsNoTracking()
            .OrderByDescending(n => n.CreatedAt)
            .Take(5)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead,
                ProductId = n.ProductId
            })
            .ToListAsync();

        return new DashboardSummaryDto
        {
            TotalRevenue = totalRevenue,
            TotalOrders = totalOrders,
            TotalProducts = totalProducts,
            TotalCustomers = totalCustomers,
            RecentOrders = recentOrders,
            RecentNotifications = recentNotifications
        };
    }

    public async Task<string> ExportSalesCsvAsync(SalesReportRequest request)
    {
        var report = await GetSalesReportAsync(request);

        var sb = new StringBuilder();
        sb.AppendLine("Date,Revenue,OrderCount");
        foreach (var day in report.DailySales)
            sb.AppendLine($"{day.Date:yyyy-MM-dd},{day.Revenue},{day.OrderCount}");

        sb.AppendLine();
        sb.AppendLine("ProductId,ProductName,QuantitySold,Revenue");
        foreach (var product in report.TopProducts)
            sb.AppendLine($"{product.ProductId},\"{product.ProductName.Replace("\"", "\"\"")}\",{product.QuantitySold},{product.Revenue}");

        return sb.ToString();
    }
}
