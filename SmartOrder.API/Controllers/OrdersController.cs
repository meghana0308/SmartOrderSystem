using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOrder.API.Models.DTOs.Orders;
using SmartOrder.API.Services;
using SmartOrder.API.Services.Interfaces;
using System.Security.Claims;

namespace SmartOrder.API.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var orderId = await _orderService.CreateOrderAsync(userId, dto);

        return Ok(new
        {
            message = "Order created successfully",
            orderId
        });
    }


    [HttpGet("my")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> MyOrders([FromQuery] OrderQueryDto query)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _orderService.GetMyOrdersAsync(userId, query));
    }

    [HttpGet("created")]
    [Authorize(Roles = "SalesExecutive")]
    public async Task<IActionResult> CreatedOrders([FromQuery] OrderQueryDto query)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await _orderService.GetCreatedOrdersAsync(userId, query));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, UpdateOrderDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _orderService.UpdateOrderAsync(id, userId, dto);
        return Ok(new { message = "Order updated successfully" });
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _orderService.CancelOrderAsync(id, userId);
        return Ok(new { message = "Order cancelled successfully" });
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "WarehouseManager")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        await _orderService.UpdateOrderStatusAsync(id, userId, dto.Status);

        return Ok(new
        {
            message = $"Order status updated to {dto.Status}"
        });
    }


}
