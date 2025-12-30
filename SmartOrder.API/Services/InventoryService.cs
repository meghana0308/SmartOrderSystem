using Microsoft.EntityFrameworkCore;
using SmartOrder.API.Data;
using SmartOrder.API.Models.DTOs.Inventory;
using SmartOrder.API.Services.Interfaces;

namespace SmartOrder.API.Services;

public class InventoryService : IInventoryService
{
    private readonly ApplicationDbContext _context;

    public InventoryService(ApplicationDbContext context)
    {
        _context = context;
    }
   

    public async Task<List<InventoryProductDto>> GetAllInventoryProductsAsync()
    {
        return await _context.Products
            .Where(p => !p.IsDeleted)
            .Include(p => p.Category)
            .Select(p => new InventoryProductDto
            {
                ProductId = p.Id,
                Name = p.Name,
                CategoryName = p.Category!.Name,
                StockQuantity = p.StockQuantity,
                ReorderLevel = p.ReorderLevel
            })
            .ToListAsync();
    }

    public async Task<List<InventoryProductDto>> GetLowStockProductsAsync()
    {
        return await _context.Products
            .Where(p => !p.IsDeleted && p.StockQuantity <= p.ReorderLevel)
            .Include(p => p.Category)
            .Select(p => new InventoryProductDto
            {
                ProductId = p.Id,
                Name = p.Name,
                CategoryName = p.Category!.Name,
                StockQuantity = p.StockQuantity,
                ReorderLevel = p.ReorderLevel
            })
            .ToListAsync();
    }

    public async Task UpdateStockAsync(string userId, int productId, UpdateStockDto dto)
    {
        if (!await UserInRoleAsync(userId, "WarehouseManager"))
            throw new UnauthorizedAccessException("Only WarehouseManager can update inventory");

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);

        if (product == null)
            throw new KeyNotFoundException("Product not found");

        if (dto.StockQuantity < 0)
            throw new ArgumentException("Stock cannot be negative");

        product.StockQuantity = dto.StockQuantity;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task UpdateReorderLevelAsync(string userId, int productId, UpdateReorderLevelDto dto)

    {
        if (!await UserInRoleAsync(userId, "WarehouseManager"))
            throw new UnauthorizedAccessException("Only WarehouseManager can update inventory");

        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);

        if (product == null)
            throw new KeyNotFoundException("Product not found");

        if (dto.ReorderLevel < 0)
            throw new ArgumentException("Reorder level cannot be negative");

        product.ReorderLevel = dto.ReorderLevel;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
    private async Task<bool> UserInRoleAsync(string userId, string role)
    {
        return await _context.UserRoles.AnyAsync(ur =>
            ur.UserId == userId &&
            _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == role));
    }

}
