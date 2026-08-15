using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HeritageMarket.Application.Common;
using HeritageMarket.Application.DTOs;
using HeritageMarket.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeritageMarket.Web.Controllers.Api;

public class AddToCartApiRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class UpdateCartItemApiRequest
{
    public int Quantity { get; set; }
}

[ApiController]
[Route("api/cart")]
[Produces("application/json")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CartApiController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartApiController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private string? CurrentUserId =>
        User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    /// <summary>Gets the authenticated user's cart.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCart()
    {
        if (CurrentUserId is null) return Unauthorized();
        return Ok(await _cartService.GetCartAsync(CurrentUserId));
    }

    /// <summary>Adds a product to the authenticated user's cart.</summary>
    [HttpPost("items")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddItem(AddToCartApiRequest request)
    {
        if (CurrentUserId is null) return Unauthorized();

        try
        {
            await _cartService.AddToCartAsync(CurrentUserId, request.ProductId, request.Quantity);
            return Ok(await _cartService.GetCartAsync(CurrentUserId));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Updates the quantity of a cart item.</summary>
    [HttpPut("items/{cartItemId:int}")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateItem(int cartItemId, UpdateCartItemApiRequest request)
    {
        if (CurrentUserId is null) return Unauthorized();

        try
        {
            await _cartService.UpdateQuantityAsync(CurrentUserId, cartItemId, request.Quantity);
            return Ok(await _cartService.GetCartAsync(CurrentUserId));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Removes an item from the authenticated user's cart.</summary>
    [HttpDelete("items/{cartItemId:int}")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveItem(int cartItemId)
    {
        if (CurrentUserId is null) return Unauthorized();

        await _cartService.RemoveItemAsync(CurrentUserId, cartItemId);
        return Ok(await _cartService.GetCartAsync(CurrentUserId));
    }
}
