using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.ViewComponents;

public class WishlistSummaryViewComponent : ViewComponent
{
    private readonly IWishlistService _wishlistService;
    private readonly UserManager<ApplicationUser> _userManager;

    public WishlistSummaryViewComponent(IWishlistService wishlistService, UserManager<ApplicationUser> userManager)
    {
        _wishlistService = wishlistService;
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var principal = HttpContext.User;
        if (!(principal.Identity?.IsAuthenticated ?? false))
            return View(0);

        var userId = _userManager.GetUserId(principal);
        if (userId is null) return View(0);

        var ids = await _wishlistService.GetWishlistedProductIdsAsync(userId);
        return View(ids.Count);
    }
}
