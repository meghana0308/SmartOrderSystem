using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOrder.API.Models.DTOs.Product;
using SmartOrder.API.Services;
using System.Security.Claims;

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
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto dto)

    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var id = await _productService.CreateCategoryAsync(userId, dto);
        return Ok(new { CategoryId = id });
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        return Ok(await _productService.GetAllCategoriesAsync());
    }

    [HttpPut("categories/{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _productService.UpdateCategoryAsync(userId, id, dto);
        return NoContent();
    }

    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _productService.DeleteCategoryAsync(userId, id);
        return NoContent();
    }
}
