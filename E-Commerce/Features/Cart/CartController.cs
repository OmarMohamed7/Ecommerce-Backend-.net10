using System.Security.Claims;
using E_Commerce.Common;
using E_Commerce.Features.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Features.Cart;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartController(ICartService cartService) : ControllerBase
{
    [Authorize(Roles = Roles.Admin)]
    [HttpGet("All")]
    public async Task<IActionResult> GetAllCarts([FromQuery] CartQueryParams queryParams)
    {
        var carts = await cartService.GetAllCarts(queryParams);

        return Ok(ApiResponse<PagedResult<Cart>>.Ok(carts));
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = GetCurrentUserId();
        var cart = await cartService.GetCartAsync(userId);

        return Ok(ApiResponse<CartDto>.Ok(cart));
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddToCartDto addToCartDto)
    {
        var userId = GetCurrentUserId();

        var res = await cartService.AddToCartAsync(userId, addToCartDto);

        return Ok(ApiResponse<CartDto>.Ok(res));
    }

    [HttpPut("items/{productId}")]
    public async Task<IActionResult> UpdateItem(int productId, [FromBody] UpdateCartItemDto updateCartItemDto)
    {
        var userId = GetCurrentUserId();

        var cart = await cartService.UpdateCartItemAsync(userId, productId, updateCartItemDto);

        if (cart == null) return NotFound(ApiResponse.Fail("Something went wrong"));

        return Ok(ApiResponse<CartDto>.Ok(cart));
    }

    [HttpDelete("items/{productId}")]
    public async Task<IActionResult> DeleteItem(int productId)
    {
        var userId = GetCurrentUserId();

        var res = await cartService.RemoveFromCartAsync(userId, productId);

        if (!res) return NotFound(ApiResponse.Fail("Something went wrong"));

        return NoContent();
    }

    private int GetCurrentUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(userId ?? string.Empty);
    }
}