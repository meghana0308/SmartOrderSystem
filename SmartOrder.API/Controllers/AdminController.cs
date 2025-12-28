using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOrder.API.Models.DTOs.Product;
using SmartOrder.API.Services;

namespace SmartOrder.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IProductService _productService;

    public AdminController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory(CategoryCreateDto dto)
    {
        var id = await _productService.CreateCategoryAsync(dto);
        return Ok(new { CategoryId = id });
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        return Ok(await _productService.GetAllCategoriesAsync());
    }

    [HttpPut("categories/{id}")]
    public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateDto dto)
    {
        await _productService.UpdateCategoryAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await _productService.DeleteCategoryAsync(id);
        return NoContent();
    }
}
