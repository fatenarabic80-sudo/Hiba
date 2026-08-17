using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Domain.Enums;
using HeritageMarket.Infrastructure.Identity;
using HeritageMarket.Web.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentityRoles.Admin)]
public class BookRequestsController : Controller
{
    private readonly IBookAccessService _bookAccessService;

    public BookRequestsController(IBookAccessService bookAccessService)
    {
        _bookAccessService = bookAccessService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _bookAccessService.GetAllAsync());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(BookRequestReviewViewModel model)
    {
        await _bookAccessService.ReviewAsync(model.Id, BookAccessStatus.Approved, model.AdminNote);
        TempData["StatusMessage"] = "Request approved — the customer can now browse Heritage Books.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(BookRequestReviewViewModel model)
    {
        await _bookAccessService.ReviewAsync(model.Id, BookAccessStatus.Rejected, model.AdminNote);
        TempData["StatusMessage"] = "Request rejected.";
        return RedirectToAction(nameof(Index));
    }
}
