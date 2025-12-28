using Microsoft.EntityFrameworkCore;
using SmartOrder.API.Data;
using SmartOrder.API.Models.DTOs.Product;
using SmartOrder.API.Models.Entities;

namespace SmartOrder.API.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateCategoryAsync(CategoryCreateDto dto)
    {
        var category = new Category
        {
            Name = dto.Name
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return category.Id;
    }

    public async Task<List<CategoryResponseDto>> GetAllCategoriesAsync()
    {
        return await _context.Categories
            .Select(c => new CategoryResponseDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();
    }

    public async Task UpdateCategoryAsync(int id, CategoryUpdateDto dto)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (category == null)
            throw new KeyNotFoundException("Category not found");

        // Update only if a new name is provided
        if (!string.IsNullOrWhiteSpace(dto.Name))
            category.Name = dto.Name;

        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }


    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (category == null)
            throw new KeyNotFoundException("Category not found");

        var hasProducts = await _context.Products
            .AnyAsync(p => p.CategoryId == id && !p.IsDeleted);

        if (hasProducts)
            throw new InvalidOperationException("Cannot delete category with existing products");

        category.IsDeleted = true;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task<int> CreateProductAsync(CreateProductDto dto)
    {
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == dto.CategoryId && !c.IsDeleted);

        if (!categoryExists)
            throw new Exception("Invalid category");

        var product = new Product
        {
            Name = dto.Name,
            UnitPrice = dto.UnitPrice,
            CategoryId = dto.CategoryId,
            StockQuantity = dto.StockQuantity,
            ReorderLevel = dto.ReorderLevel
        };


        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product.Id;
    }

    public async Task UpdateProductAsync(int id, UpdateProductDto dto)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (product == null)
            throw new KeyNotFoundException("Product not found");

        // Update category only if a valid CategoryId is provided (non-zero)
        if (dto.CategoryId > 0)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == dto.CategoryId && !c.IsDeleted);

            if (!categoryExists)
                throw new ArgumentException("Invalid CategoryId");

            product.CategoryId = dto.CategoryId;
        }

        // Update only if the new value is provided (not null/empty/default)
        if (!string.IsNullOrWhiteSpace(dto.Name))
            product.Name = dto.Name;

        if (dto.UnitPrice > 0)
            product.UnitPrice = dto.UnitPrice;

        if (dto.StockQuantity > 0)
            product.StockQuantity = dto.StockQuantity;

        if (dto.ReorderLevel > 0)
            product.ReorderLevel = dto.ReorderLevel;

        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }




    public async Task DeleteProductAsync(int id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            throw new Exception("Product not found");

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

    public async Task<List<ProductResponseDto>> GetLowStockProductsAsync()
    {
        return await _context.Products
            .Where(p => !p.IsDeleted && p.StockQuantity <= p.ReorderLevel)
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
