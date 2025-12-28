using Microsoft.EntityFrameworkCore;
using SmartOrder.API.Data;
using SmartOrder.API.Enums;
using SmartOrder.API.Models.DTOs.Orders;
using SmartOrder.API.Models.Entities;
using SmartOrder.API.Services.Interfaces;

namespace SmartOrder.API.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateOrderAsync(string userId, CreateOrderDto dto)
    {
        if (dto.Items.Count == 0)
            throw new ArgumentException("Order must contain at least one item");

        var isSales = await _context.UserRoles
            .AnyAsync(ur => ur.UserId == userId &&
                _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Sales"));

        var customerId = isSales
            ? dto.CustomerId ?? throw new ArgumentException("CustomerId is required for Sales orders")
            : userId;

        var customerExists = await _context.Users.AnyAsync(u => u.Id == customerId);
        if (!customerExists)
            throw new ArgumentException("Invalid CustomerId");

        var order = new Order
        {
            CustomerId = customerId,
            CreatedByUserId = userId,
            Status = OrderStatus.Created
        };


        decimal total = 0;

        foreach (var item in dto.Items)
        {
            if (item.Quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero");

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == item.ProductId && !p.IsDeleted);

            if (product == null)
                throw new KeyNotFoundException($"Product {item.ProductId} not found");

            if (product.StockQuantity < item.Quantity)
                throw new InvalidOperationException($"Insufficient stock for {product.Name}");

            product.StockQuantity -= item.Quantity;

            order.OrderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.UnitPrice
            });

            total += product.UnitPrice * item.Quantity;

        }

        order.TotalAmount = total;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return order.Id;
    }

    public async Task<List<OrderListDto>> GetMyOrdersAsync(string userId, OrderQueryDto query)
    {
        var orders = _context.Orders
            .Where(o => o.CustomerId == userId);

        if (!string.IsNullOrEmpty(query.Status))
            if (Enum.TryParse<OrderStatus>(query.Status, true, out var status))
                orders = orders.Where(o => o.Status == status);


        return await orders
            .OrderByDescending(o => o.OrderDate)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(o => new OrderListDto
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status.ToString(),
                TotalAmount = o.TotalAmount
            })
            .ToListAsync();
    }

    public async Task<List<OrderListDto>> GetCreatedOrdersAsync(string userId, OrderQueryDto query)
    {
        var orders = _context.Orders
            .Where(o => o.CreatedByUserId == userId);

        if (!string.IsNullOrEmpty(query.Status))
            if (Enum.TryParse<OrderStatus>(query.Status, true, out var status))
                orders = orders.Where(o => o.Status == status);


        return await orders
            .OrderByDescending(o => o.OrderDate)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(o => new OrderListDto
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status.ToString(),
                TotalAmount = o.TotalAmount
            })
            .ToListAsync();
    }

    public async Task UpdateOrderAsync(int orderId, string userId, UpdateOrderDto dto)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            throw new KeyNotFoundException("Order not found");

        if (order.Status != OrderStatus.Created)
            throw new InvalidOperationException("Only orders with status 'Created' can be edited");

        if (order.CreatedByUserId != userId && order.CustomerId != userId)
            throw new UnauthorizedAccessException("You can only edit your own orders");

        // Restore stock for existing items
        foreach (var item in order.OrderItems)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
            if (product != null)
                product.StockQuantity += item.Quantity;
        }

        order.OrderItems.Clear();
        decimal total = 0;

        foreach (var item in dto.Items)
        {
            if (item.Quantity <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero");

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == item.ProductId && !p.IsDeleted);

            if (product == null)
                throw new KeyNotFoundException($"Product {item.ProductId} not found");

            if (product.StockQuantity < item.Quantity)
                throw new InvalidOperationException($"Insufficient stock for {product.Name}");

            product.StockQuantity -= item.Quantity;

            order.OrderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.UnitPrice
            });

            total += product.UnitPrice * item.Quantity;
        }

        order.TotalAmount = total;
        order.OrderDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task CancelOrderAsync(int orderId, string userId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            throw new KeyNotFoundException("Order not found");

        if (order.Status != OrderStatus.Created)
            throw new InvalidOperationException("Only orders with status 'Created' can be cancelled");

        if (order.CreatedByUserId != userId && order.CustomerId != userId)
            throw new UnauthorizedAccessException("You can only cancel your own orders");

        // Restore stock
        foreach (var item in order.OrderItems)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
            if (product != null)
                product.StockQuantity += item.Quantity;
        }

        order.Status = OrderStatus.Cancelled;
        await _context.SaveChangesAsync();
    }


}
