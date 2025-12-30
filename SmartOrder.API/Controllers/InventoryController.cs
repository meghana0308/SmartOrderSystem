using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOrder.API.Models.DTOs.Inventory;
using SmartOrder.API.Services;
using SmartOrder.API.Services.Interfaces;
using System.Security.Claims;

[ApiController]
[Route("api/inventory")]
[Authorize(Roles = "WarehouseManager")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetAllInventoryProducts()
    {
        return Ok(await _inventoryService.GetAllInventoryProductsAsync());
    }

    [HttpGet("lowstock")]
    public async Task<IActionResult> GetLowStockProducts()
    {
        return Ok(await _inventoryService.GetLowStockProductsAsync());
    }

    [HttpPut("{productId}/stock")]
    public async Task<IActionResult> UpdateStock(int productId, [FromBody] UpdateStockDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User not authenticated");

        await _inventoryService.UpdateStockAsync(userId, productId, dto);
        return NoContent();
    }

    [HttpPut("{productId}/reorder-level")]
    public async Task<IActionResult> UpdateReorderLevel(int productId, [FromBody] UpdateReorderLevelDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User not authenticated");

        await _inventoryService.UpdateReorderLevelAsync(userId, productId, dto);
        return NoContent();
    }

}
