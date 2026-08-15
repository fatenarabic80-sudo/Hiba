using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentityRoles.Admin)]
public class DashboardController : Controller
{
    private readonly IReportService _reportService;

    public DashboardController(IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<IActionResult> Index()
    {
        var summary = await _reportService.GetDashboardSummaryAsync();
        return View(summary);
    }
}
