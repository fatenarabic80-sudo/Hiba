using System.Text;
using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentityRoles.Admin)]
public class ReportsController : Controller
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
    {
        var request = new SalesReportRequest
        {
            FromDate = fromDate ?? DateTime.UtcNow.AddDays(-30),
            ToDate = toDate ?? DateTime.UtcNow
        };

        var report = await _reportService.GetSalesReportAsync(request);
        ViewData["FromDate"] = request.FromDate;
        ViewData["ToDate"] = request.ToDate;

        return View(report);
    }

    public async Task<IActionResult> ExportCsv(DateTime? fromDate, DateTime? toDate)
    {
        var request = new SalesReportRequest
        {
            FromDate = fromDate ?? DateTime.UtcNow.AddDays(-30),
            ToDate = toDate ?? DateTime.UtcNow
        };

        var csv = await _reportService.ExportSalesCsvAsync(request);
        var bytes = Encoding.UTF8.GetBytes(csv);

        return File(bytes, "text/csv", $"sales-report-{DateTime.UtcNow:yyyyMMdd}.csv");
    }
}
