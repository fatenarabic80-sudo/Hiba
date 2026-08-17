using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Infrastructure.Identity;
using HeritageMarket.Web.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<CheckoutController> _logger;

    public CheckoutController(
        ICartService cartService,
        IOrderService orderService,
        UserManager<ApplicationUser> userManager,
        ILogger<CheckoutController> logger)
    {
        _cartService = cartService;
        _orderService = orderService;
        _userManager = userManager;
        _logger = logger;
    }

    private string CurrentUserId => _userManager.GetUserId(User)!;

    public async Task<IActionResult> Index()
    {
        var cart = await _cartService.GetCartAsync(CurrentUserId);
        if (cart.Items.Count == 0)
        {
            TempData["ErrorMessage"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        var user = await _userManager.GetUserAsync(User);

        var model = new CheckoutViewModel
        {
            Cart = cart,
            ShippingAddress = user?.Address ?? string.Empty
        };

        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CheckoutViewModel model)
    {
        var cart = await _cartService.GetCartAsync(CurrentUserId);
        model.Cart = cart;

        if (cart.Items.Count == 0)
        {
            TempData["ErrorMessage"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var request = model.Adapt<PlaceOrderRequest>();
            request.ApplicationUserId = CurrentUserId;

            var order = await _orderService.PlaceOrderAsync(request);

            return RedirectToAction(nameof(Confirmation), new { id = order.Id });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Checkout failed for user {UserId}", CurrentUserId);
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    public async Task<IActionResult> Confirmation(int id)
    {
        var order = await _orderService.GetOrderDetailAsync(id, CurrentUserId);
        if (order is null) return NotFound();

        return View(order);
    }
}
