using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOrder.API.Models.DTOs.Product;
using SmartOrder.API.Services;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        var id = await _service.CreateProductAsync(dto);
        return Ok(id);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateProductDto dto)
    {
        await _service.UpdateProductAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteProductAsync(id);
        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllProductsAsync());
    }

    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        return Ok(await _service.GetProductsByCategoryAsync(categoryId));
    }

    [HttpGet("lowstock")]
    public async Task<IActionResult> GetLowStock()
    {
        var products = await _service.GetLowStockProductsAsync();
        return Ok(products);
    }

}
