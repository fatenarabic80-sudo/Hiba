using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Controllers.Api;

public class PlaceOrderApiRequest
{
    public string ShippingAddress { get; set; } = string.Empty;
    public string ShippingCity { get; set; } = string.Empty;
}

[ApiController]
[Route("api/orders")]
[Produces("application/json")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class OrdersApiController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;

    public OrdersApiController(IOrderService orderService, ICartService cartService)
    {
        _orderService = orderService;
        _cartService = cartService;
    }

    private string? CurrentUserId =>
        User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    /// <summary>Lists the authenticated user's orders.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrderListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrders()
    {
        if (CurrentUserId is null) return Unauthorized();
        return Ok(await _orderService.GetOrdersForUserAsync(CurrentUserId));
    }

    /// <summary>Gets a single order belonging to the authenticated user.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrder(int id)
    {
        if (CurrentUserId is null) return Unauthorized();

        var order = await _orderService.GetOrderDetailAsync(id, CurrentUserId);
        return order is null ? NotFound() : Ok(order);
    }

    /// <summary>Places an order from the authenticated user's current cart.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PlaceOrder(PlaceOrderApiRequest request)
    {
        if (CurrentUserId is null) return Unauthorized();

        try
        {
            var order = await _orderService.PlaceOrderAsync(new PlaceOrderRequest
            {
                ApplicationUserId = CurrentUserId,
                ShippingAddress = request.ShippingAddress,
                ShippingCity = request.ShippingCity
            });

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
