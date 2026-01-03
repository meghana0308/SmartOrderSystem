using SmartOrder.API.Enums;
using SmartOrder.API.Models.Entities;

namespace SmartOrder.API.Services.Interfaces
{
    public interface INotificationService
    {
        Task NotifyWarehouseManagersLowStockAsync(Product product);
        Task<List<Notification>> GetMyNotificationsAsync(string userId);
        Task MarkAsReadAsync(int notificationId, string userId);
        Task NotifyOrderStatusChangedAsync(Order order, OrderStatus oldStatus);

    }


}
