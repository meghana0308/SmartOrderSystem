using Microsoft.EntityFrameworkCore;
using SmartOrder.API.Data;
using SmartOrder.API.Enums;
using SmartOrder.API.Models.Entities;
using SmartOrder.API.Services.Interfaces;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _context;

    public NotificationService(ApplicationDbContext context)
    {
        _context = context;
    }

    // 🔔 LOW STOCK (Warehouse only)
    public async Task NotifyWarehouseManagersLowStockAsync(Product product)
    {
        if (product.StockQuantity > product.ReorderLevel)
            return;

        var warehouseManagers = await _context.UserRoles
            .Where(ur => _context.Roles.Any(r =>
                r.Id == ur.RoleId && r.Name == "WarehouseManager"))
            .Select(ur => ur.UserId)
            .ToListAsync();

        foreach (var userId in warehouseManagers)
        {
            bool exists = await _context.Notifications.AnyAsync(n =>
                n.UserId == userId &&
                n.Title == "Low Stock Alert" &&
                n.Message.Contains(product.Name) &&
                !n.IsRead);

            if (exists) continue;

            _context.Notifications.Add(new Notification
            {
                UserId = userId,
                Title = "Low Stock Alert",
                Message = $"Low stock alert: {product.Name} (Qty: {product.StockQuantity})"
            });
        }

        await _context.SaveChangesAsync();
    }

    // 🔔 ORDER STATUS CHANGE (Customer + Sales)
    public async Task NotifyOrderStatusChangedAsync(Order order, OrderStatus oldStatus)
    {
        var recipients = new HashSet<string>
        {
            order.CustomerId,
            order.CreatedByUserId
        };

        foreach (var userId in recipients)
        {
            _context.Notifications.Add(new Notification
            {
                UserId = userId,
                Title = "Order Status Updated",
                Message = $"Order #{order.Id} status changed from {oldStatus} to {order.Status}"
            });
        }

        await _context.SaveChangesAsync();
    }

    // 📥 GET MY NOTIFICATIONS
    public async Task<List<Notification>> GetMyNotificationsAsync(string userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    // ✅ MARK READ
    public async Task MarkAsReadAsync(int notificationId, string userId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

        if (notification == null) return;

        notification.IsRead = true;
        await _context.SaveChangesAsync();
    }
}
