using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOrder.API.Models.DTOs.Orders;
using SmartOrder.API.Services.Interfaces;
using SmartOrder.API.Helpers;
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
    [Authorize(Roles = "Customer,SalesExecutive")]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                throw new AppException("Invalid data", 400);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var orderId = await _orderService.CreateOrderAsync(userId, dto);

            return Ok(new
            {
                message = "Order created successfully",
                orderId
            });
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }


    [HttpGet("my")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> MyOrders([FromQuery] OrderQueryDto query)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return Ok(await _orderService.GetMyOrdersAsync(userId, query));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpGet("created")]
    [Authorize(Roles = "SalesExecutive")]
    public async Task<IActionResult> CreatedOrders([FromQuery] OrderQueryDto query)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return Ok(await _orderService.GetCreatedOrdersAsync(userId, query));
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Customer,SalesExecutive")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await _orderService.UpdateOrderAsync(id, userId, dto);

            return Ok(new { message = "Order updated successfully" });
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpPut("{id}/cancel")]
    [Authorize(Roles = "Customer,SalesExecutive")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await _orderService.CancelOrderAsync(id, userId);

            return Ok(new { message = "Order cancelled successfully" });
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "WarehouseManager")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDto dto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await _orderService.UpdateOrderStatusAsync(id, userId, dto.Status);

            return Ok(new
            {
                message = $"Order status updated to {dto.Status}"
            });
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }
    [HttpGet("all")]
    [Authorize(Roles = "WarehouseManager")]
    public async Task<IActionResult> GetAllOrders([FromQuery] OrderQueryDto query)
    {
        return Ok(await _orderService.GetAllOrdersAsync(query));
    }

}
