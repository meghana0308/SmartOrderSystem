using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartOrder.API.Models.Entities;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "SalesExecutive")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomers()
    {
        var users = _userManager.Users.ToList();
        var result = new List<object>();

        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            if (roles.Contains("Customer"))
            {
                result.Add(new
                {
                    userId = u.Id,
                    fullName = u.FullName,
                    email = u.Email
                });
            }
        }

        return Ok(result);
    }
}
