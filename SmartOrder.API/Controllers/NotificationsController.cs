using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartOrder.API.Services.Interfaces;
using System.Security.Claims;

namespace SmartOrder.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize(Roles = "WarehouseManager,Customer,SalesExecutive")]
 
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationsController(INotificationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> MyNotifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return Ok(await _service.GetMyNotificationsAsync(userId));
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkRead(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _service.MarkAsReadAsync(id, userId);
            return NoContent();
        }
    }

}
