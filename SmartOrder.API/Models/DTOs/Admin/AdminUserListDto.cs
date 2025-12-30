namespace SmartOrder.API.Models.DTOs.Admin;

public class AdminUserListDto
{
    public string UserId { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? FullName { get; set; }
    public string Role { get; set; } = null!;
}
