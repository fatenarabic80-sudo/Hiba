using HeritageMarket.Application.DTOs;

namespace HeritageMarket.Application.Services.Interfaces;

public interface IReportService
{
    Task<SalesReportDto> GetSalesReportAsync(SalesReportRequest request);
    Task<DashboardSummaryDto> GetDashboardSummaryAsync();
    Task<string> ExportSalesCsvAsync(SalesReportRequest request);
}
