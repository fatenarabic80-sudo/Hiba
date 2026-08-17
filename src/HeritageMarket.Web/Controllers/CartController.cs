using HeritageMarket.Application.Common;
using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartController(ICartService cartService, UserManager<ApplicationUser> userManager)
    {
        _cartService = cartService;
        _userManager = userManager;
    }

    private string CurrentUserId => _userManager.GetUserId(User)!;

    public async Task<IActionResult> Index()
    {
        var cart = await _cartService.GetCartAsync(CurrentUserId);
        return View(cart);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity, string? selectedSize, string? returnUrl)
    {
        try
        {
            await _cartService.AddToCartAsync(CurrentUserId, productId, quantity <= 0 ? 1 : quantity, selectedSize);
            TempData["StatusMessage"] = "Item added to your cart.";
        }
        catch (InsufficientStockException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (ValidationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (NotFoundException)
        {
            TempData["ErrorMessage"] = "That product could not be found.";
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
    {
        try
        {
            await _cartService.UpdateQuantityAsync(CurrentUserId, cartItemId, quantity <= 0 ? 1 : quantity);
        }
        catch (InsufficientStockException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (NotFoundException)
        {
            TempData["ErrorMessage"] = "That cart item no longer exists.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int cartItemId)
    {
        await _cartService.RemoveItemAsync(CurrentUserId, cartItemId);
        TempData["StatusMessage"] = "Item removed from your cart.";
        return RedirectToAction(nameof(Index));
    }
}
