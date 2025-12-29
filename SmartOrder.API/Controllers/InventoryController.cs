using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOrder.API.Models.DTOs.Inventory;
using SmartOrder.API.Services;
using SmartOrder.API.Services.Interfaces;

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
    public async Task<IActionResult> UpdateStock(
        int productId,
        UpdateStockDto dto)
    {
        await _inventoryService.UpdateStockAsync(productId, dto);
        return NoContent();
    }

    [HttpPut("{productId}/reorder-level")]
    public async Task<IActionResult> UpdateReorderLevel(
        int productId,
        UpdateReorderLevelDto dto)
    {
        await _inventoryService.UpdateReorderLevelAsync(productId, dto);
        return NoContent();
    }
}
