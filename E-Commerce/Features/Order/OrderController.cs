using E_Commerce.Common;
using E_Commerce.Extensions;
using E_Commerce.Features.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Features.Order;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrderController(
    IOrderService orderService,
    IValidator<UpdateOrderStatusDto> statusValidator) : ControllerBase
{
    [HttpGet("All")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetAllOrders([FromQuery] OrderDtoQueryParams queryParams)
    {
        var res = await orderService.GetAllOrdersAsync(queryParams);

        return Ok(ApiResponse<PagedResult<OrderDto>>.Ok(res));
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders([FromQuery] OrderDtoQueryParams queryParams)
    {
        var res = await orderService.GetMyOrdersAsync(queryParams.Page, queryParams.PageSize);

        return Ok(ApiResponse<PagedResult<OrderDto>>.Ok(res));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetOrder(int id)
    {
        var order = await orderService.GetOrderByIdAsync(id);

        return order == null
            ? NotFound(ApiResponse.Fail("Order not found."))
            : Ok(ApiResponse<OrderDto>.Ok(order));
    }

    // Turns the current user's cart into an order
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout()
    {
        var order = await orderService.CheckoutAsync();

        return order == null
            ? BadRequest(ApiResponse.Fail("Your cart is empty."))
            : CreatedAtAction(nameof(GetOrder), new { id = order.Id }, ApiResponse<OrderDto>.Ok(order));
    }

    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
    {
        var validationResult = await statusValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
            return BadRequest(ApiResponse<OrderDto>.Fail(validationResult.ToApiErrors()));

        var (order, error) = await orderService.UpdateOrderStatusAsync(id, dto.Status);

        if (error != null) return BadRequest(ApiResponse.Fail(error));

        return order == null
            ? NotFound(ApiResponse.Fail("Order not found."))
            : Ok(ApiResponse<OrderDto>.Ok(order));
    }
}