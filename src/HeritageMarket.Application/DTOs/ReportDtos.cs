namespace HeritageMarket.Application.DTOs;

public class SalesReportRequest
{
    public DateTime FromDate { get; set; } = DateTime.UtcNow.AddDays(-30);
    public DateTime ToDate { get; set; } = DateTime.UtcNow;
}

public class SalesReportDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public List<DailySalesDto> DailySales { get; set; } = new();
    public List<TopProductDto> TopProducts { get; set; } = new();
}

public class DailySalesDto
{
    public DateTime Date { get; set; }
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
}

public class TopProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
}

public class DashboardSummaryDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int TotalProducts { get; set; }
    public int TotalCustomers { get; set; }
    public List<NotificationDto> RecentNotifications { get; set; } = new();
    public List<OrderListItemDto> RecentOrders { get; set; } = new();
}
