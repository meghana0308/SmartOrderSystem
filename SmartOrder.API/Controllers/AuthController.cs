using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartOrder.API.Helpers;
using SmartOrder.API.Models.DTOs.Auth;
using SmartOrder.API.Models.Entities;

namespace SmartOrder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            JwtTokenGenerator jwtTokenGenerator)
        {
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        // NOTE:
        // In this system, Email is used as the Username.
        // Users authenticate using Email + Password.
        // No separate username is supported.

        // ---------------- REGISTER ----------------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return BadRequest("User already exists");

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Default role = Customer
            await _userManager.AddToRoleAsync(user, "Customer");

            return Ok("Registration successful");
        }

        // ---------------- LOGIN ----------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!passwordValid)
                return Unauthorized("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            return Ok(new AuthResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            });
        }
    }
}
