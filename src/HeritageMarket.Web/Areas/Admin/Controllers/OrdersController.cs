using HeritageMarket.Application.Services.Interfaces;
using HeritageMarket.Domain.Enums;
using HeritageMarket.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentityRoles.Admin)]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public async Task<IActionResult> Index(OrderStatus? status, int pageNumber = 1)
    {
        var orders = await _orderService.GetAllOrdersAsync(pageNumber, 15, status);
        ViewData["StatusFilter"] = status;
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _orderService.GetOrderDetailAsync(id);
        if (order is null) return NotFound();

        return View(order);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
    {
        await _orderService.UpdateStatusAsync(id, status);
        TempData["StatusMessage"] = $"Order #{id} marked as {status}.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
