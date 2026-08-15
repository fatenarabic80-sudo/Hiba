using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Controllers;

[Authorize]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrdersController(IOrderService orderService, UserManager<ApplicationUser> userManager)
    {
        _orderService = orderService;
        _userManager = userManager;
    }

    private string CurrentUserId => _userManager.GetUserId(User)!;

    public async Task<IActionResult> Index()
    {
        var orders = await _orderService.GetOrdersForUserAsync(CurrentUserId);
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _orderService.GetOrderDetailAsync(id, CurrentUserId);
        if (order is null) return NotFound();

        return View(order);
    }
}
