using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Domain.Enums;
using HeritageMarket.Infrastructure.Identity;
using HeritageMarket.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Controllers;

/// <summary>
/// The customer's personal dashboard, showing only data relevant to the signed-in user —
/// distinct from the Admin Dashboard, which shows store-wide data.
/// </summary>
[Authorize]
public class DashboardController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IWishlistService _wishlistService;
    private readonly ICartService _cartService;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(
        IOrderService orderService,
        IWishlistService wishlistService,
        ICartService cartService,
        UserManager<ApplicationUser> userManager)
    {
        _orderService = orderService;
        _wishlistService = wishlistService;
        _cartService = cartService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        var orders = await _orderService.GetOrdersForUserAsync(user.Id);
        var wishlist = await _wishlistService.GetWishlistAsync(user.Id);
        var cart = await _cartService.GetCartAsync(user.Id);

        var model = new DashboardViewModel
        {
            FullName = user.FullName,
            ProfileImageUrl = user.ProfileImageUrl,
            MemberSince = user.CreatedAt,
            TotalOrders = orders.Count,
            TotalSpent = orders.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.TotalAmount),
            RecentOrders = orders.Take(5).ToList(),
            WishlistCount = wishlist.Count,
            WishlistPreview = wishlist.Take(4).ToList(),
            CartItemCount = cart.Items.Sum(i => i.Quantity)
        };

        return View(model);
    }
}
