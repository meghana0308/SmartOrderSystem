using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartOrder.API.Models.DTOs.Admin;
using SmartOrder.API.Models.DTOs.Product;
using SmartOrder.API.Models.Entities;
using SmartOrder.API.Services;
using System.Security.Claims;

namespace SmartOrder.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(IProductService productService, UserManager<ApplicationUser> userManager)
    {
        _productService = productService;
        _userManager = userManager;
    }

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized("User not authenticated");

        try
        {
            var id = await _productService.CreateCategoryAsync(userId, dto);
            return Ok(new { CategoryId = id });
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, ex.Message);
        }
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
        if (string.IsNullOrEmpty(userId)) return Unauthorized("User not authenticated");

        try
        {
            await _productService.UpdateCategoryAsync(userId, id, dto);
            return NoContent();
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, ex.Message);
        }
    }

    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized("User not authenticated");

        try
        {
            await _productService.DeleteCategoryAsync(userId, id);
            return NoContent();
        }
        catch (AppException ex)
        {
            return StatusCode(ex.StatusCode, ex.Message);
        }
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = _userManager.Users.ToList();

        var result = new List<AdminUserListDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            result.Add(new AdminUserListDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Role = roles.FirstOrDefault() ?? "Customer"
            });
        }

        return Ok(result);
    }

    [HttpPut("users/{userId}/role")]
    public async Task<IActionResult> ChangeUserRole(
    string userId,
    [FromBody] ChangeUserRoleDto dto)
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (adminId == userId)
            return BadRequest("Admin cannot change own role");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound("User not found");

        var validRoles = new[]
        {
        "Customer",
        "SalesExecutive",
        "WarehouseManager",
        "FinanceOfficer"
    };

        if (!validRoles.Contains(dto.Role))
            return BadRequest("Invalid role");

        var currentRoles = await _userManager.GetRolesAsync(user);

        if (currentRoles.Any())
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

        await _userManager.AddToRoleAsync(user, dto.Role);

        return NoContent();
    }


}
