using SmartOrder.API.Models.DTOs.Product;

public interface IProductService
{
    // CATEGORY
    Task<int> CreateCategoryAsync(CategoryCreateDto dto);
    Task<List<CategoryResponseDto>> GetAllCategoriesAsync();
    Task UpdateCategoryAsync(int id, CategoryUpdateDto dto);
    Task DeleteCategoryAsync(int id);

    // PRODUCT (CATALOG ONLY)
    Task<int> CreateProductAsync(CreateProductDto dto);
    Task UpdateProductAsync(int id, UpdateProductDto dto);
    Task DeleteProductAsync(int id);

    Task<List<ProductResponseDto>> GetAllProductsAsync();
    Task<List<ProductResponseDto>> GetProductsByCategoryAsync(int categoryId);
}
