using Microsoft.AspNetCore.Identity;

namespace SmartOrder.API.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
