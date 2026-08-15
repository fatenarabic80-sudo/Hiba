using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.ViewComponents;

public class CartSummaryViewComponent : ViewComponent
{
    private readonly ICartService _cartService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartSummaryViewComponent(ICartService cartService, UserManager<ApplicationUser> userManager)
    {
        _cartService = cartService;
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var principal = HttpContext.User;
        if (!(principal.Identity?.IsAuthenticated ?? false))
            return View(0);

        var userId = _userManager.GetUserId(principal);
        if (userId is null) return View(0);

        var cart = await _cartService.GetCartAsync(userId);
        return View(cart.Items.Sum(i => i.Quantity));
    }
}
