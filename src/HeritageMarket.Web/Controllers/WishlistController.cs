using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Controllers;

[Authorize]
public class WishlistController : Controller
{
    private readonly IWishlistService _wishlistService;
    private readonly UserManager<ApplicationUser> _userManager;

    public WishlistController(IWishlistService wishlistService, UserManager<ApplicationUser> userManager)
    {
        _wishlistService = wishlistService;
        _userManager = userManager;
    }

    private string CurrentUserId => _userManager.GetUserId(User)!;

    public async Task<IActionResult> Index()
    {
        var items = await _wishlistService.GetWishlistAsync(CurrentUserId);
        ViewData["WishlistedIds"] = items.Select(i => i.Id).ToHashSet();
        return View(items);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int productId)
    {
        var wishlisted = await _wishlistService.ToggleAsync(CurrentUserId, productId);
        return Json(new { wishlisted });
    }
}
