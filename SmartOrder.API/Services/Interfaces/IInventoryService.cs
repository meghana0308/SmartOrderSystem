using SmartOrder.API.Models.DTOs.Inventory;

namespace SmartOrder.API.Services.Interfaces;

public interface IInventoryService
{
    Task<List<InventoryProductDto>> GetAllInventoryProductsAsync();
    Task<List<InventoryProductDto>> GetLowStockProductsAsync();
    Task UpdateStockAsync(int productId, UpdateStockDto dto);
    Task UpdateReorderLevelAsync(int productId, UpdateReorderLevelDto dto);
}
