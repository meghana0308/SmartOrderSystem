using SmartOrder.API.Models.DTOs.Product;

public interface IProductService
{
    // CATEGORY
    Task<int> CreateCategoryAsync(string userId, CategoryCreateDto dto);
    Task<List<CategoryResponseDto>> GetAllCategoriesAsync();
    Task UpdateCategoryAsync(string userId, int id, CategoryUpdateDto dto);
    Task DeleteCategoryAsync(string userId, int id);

    // PRODUCT (CATALOG ONLY)
    Task<int> CreateProductAsync(string userId, CreateProductDto dto);
    Task UpdateProductAsync(string userId, int id, UpdateProductDto dto);
    Task DeleteProductAsync(string userId, int id);

    Task<List<ProductResponseDto>> GetAllProductsAsync();
    Task<List<ProductResponseDto>> GetProductsByCategoryAsync(int categoryId);
}
