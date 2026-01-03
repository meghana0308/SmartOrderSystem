using Microsoft.EntityFrameworkCore;
using SmartOrder.API.Data;
using SmartOrder.API.Models.DTOs.Product;
using SmartOrder.API.Models.Entities;
using SmartOrder.API.Helpers;

namespace SmartOrder.API.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    private async Task<bool> UserInRoleAsync(string userId, string role)
    {
        return await _context.UserRoles.AnyAsync(ur =>
            ur.UserId == userId &&
            _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == role));
    }

    public async Task<int> CreateCategoryAsync(string userId, CategoryCreateDto dto)
    {
        if (!await UserInRoleAsync(userId, "Admin"))
            throw new AppException("Only Admin can manage categories", 403);

        var category = new Category { Name = dto.Name };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category.Id;
    }

    public async Task<List<CategoryResponseDto>> GetAllCategoriesAsync()
    {
        return await _context.Categories
            .Where(c => !c.IsDeleted)
            .Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();
    }

    public async Task UpdateCategoryAsync(string userId, int id, CategoryUpdateDto dto)
    {
        if (!await UserInRoleAsync(userId, "Admin"))
            throw new AppException("Only Admin can manage categories", 403);

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (category == null) throw new AppException("Category not found", 404);

        if (!string.IsNullOrWhiteSpace(dto.Name))
            category.Name = dto.Name;

        category.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(string userId, int id)
    {
        if (!await UserInRoleAsync(userId, "Admin"))
            throw new AppException("Only Admin can manage categories", 403);

        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (category == null) throw new AppException("Category not found", 404);

        var hasProducts = await _context.Products.AnyAsync(p => p.CategoryId == id && !p.IsDeleted);
        if (hasProducts)
            throw new AppException("Cannot delete category with existing products", 400);

        category.IsDeleted = true;
        category.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    // ---------- PRODUCT ----------
    public async Task<int> CreateProductAsync(string userId, CreateProductDto dto)
    {
        if (!await UserInRoleAsync(userId, "Admin"))
            throw new AppException("Only Admin can manage products", 403);

        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId && !c.IsDeleted);
        if (!categoryExists) throw new AppException("Invalid category", 400);

        var product = new Product
        {
            Name = dto.Name,
            UnitPrice = dto.UnitPrice,
            CategoryId = dto.CategoryId,
            StockQuantity = 0,
            ReorderLevel = 0
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product.Id;
    }

    public async Task UpdateProductAsync(string userId, int id, UpdateProductDto dto)
    {
        if (!await UserInRoleAsync(userId, "Admin"))
            throw new AppException("Only Admin can manage products", 403);

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (product == null) throw new AppException("Product not found", 404);

        if (dto.CategoryId > 0)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId && !c.IsDeleted);
            if (!categoryExists) throw new AppException("Invalid CategoryId", 400);
            product.CategoryId = dto.CategoryId;
        }

        if (!string.IsNullOrWhiteSpace(dto.Name)) product.Name = dto.Name;
        if (dto.UnitPrice > 0) product.UnitPrice = dto.UnitPrice;

        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(string userId, int id)
    {
        if (!await UserInRoleAsync(userId, "Admin"))
            throw new AppException("Only Admin can manage products", 403);

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) throw new AppException("Product not found", 404);

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<List<ProductResponseDto>> GetAllProductsAsync()
    {
        return await _context.Products
            .Where(p => !p.IsDeleted)
            .Include(p => p.Category)
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                UnitPrice = p.UnitPrice,
                CategoryName = p.Category!.Name
            })
            .ToListAsync();
    }

    public async Task<List<ProductResponseDto>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _context.Products
            .Where(p => !p.IsDeleted && p.CategoryId == categoryId)
            .Include(p => p.Category)
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                UnitPrice = p.UnitPrice,
                CategoryName = p.Category!.Name
            })
            .ToListAsync();
    }
}


