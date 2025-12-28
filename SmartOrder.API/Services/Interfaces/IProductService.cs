using SmartOrder.API.Models.DTOs.Product;

public interface IProductService
{
    // CATEGORY (existing)
    Task<int> CreateCategoryAsync(CategoryCreateDto dto);
    Task<List<CategoryResponseDto>> GetAllCategoriesAsync();
    Task UpdateCategoryAsync(int id, CategoryUpdateDto dto);
    Task DeleteCategoryAsync(int id);

    // PRODUCT (new)
    Task<int> CreateProductAsync(CreateProductDto dto);
    Task UpdateProductAsync(int id, UpdateProductDto dto);
    Task DeleteProductAsync(int id);
    Task<List<ProductResponseDto>> GetAllProductsAsync();
    Task<List<ProductResponseDto>> GetLowStockProductsAsync();

    Task<List<ProductResponseDto>> GetProductsByCategoryAsync(int categoryId);
}
