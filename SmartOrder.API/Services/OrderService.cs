using Microsoft.EntityFrameworkCore;
using SmartOrder.API.Data;
using SmartOrder.API.Enums;
using SmartOrder.API.Models.DTOs.Orders;
using SmartOrder.API.Models.Entities;
using SmartOrder.API.Services.Interfaces;
using SmartOrder.API.Helpers;

namespace SmartOrder.API.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;


    public OrderService(
    ApplicationDbContext context,
    INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }


    private async Task<bool> UserInRoleAsync(string userId, string role)
    {
        return await _context.UserRoles.AnyAsync(ur =>
            ur.UserId == userId &&
            _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == role));
    }

    public async Task<int> CreateOrderAsync(string userId, CreateOrderDto dto)
    {
        if (dto.Items == null || dto.Items.Count == 0)
            throw new AppException("Order must contain at least one item", 400);

        var isSales = await UserInRoleAsync(userId, "SalesExecutive");
        var isCustomer = await UserInRoleAsync(userId, "Customer");

        if (!isSales && !isCustomer)
            throw new AppException("Only Customer or SalesExecutive can create orders", 403);

        string customerId = isSales ? dto.CustomerId ?? throw new AppException("CustomerId is required for Sales orders", 400) : userId;

        var customerExists = await _context.Users.AnyAsync(u => u.Id == customerId);
        if (!customerExists)
            throw new AppException("Invalid CustomerId", 400);

        var order = new Order
        {
            CustomerId = customerId,
            CreatedByUserId = userId,
            Status = OrderStatus.Created,
            PaymentMode = dto.PaymentMode,
            PaymentStatus = dto.PaymentMode == PaymentMode.PayNow
                ? PaymentStatus.Paid
                : PaymentStatus.Pending
        };

        decimal total = 0;

        foreach (var item in dto.Items)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId && !p.IsDeleted);
            if (product == null)
                throw new AppException($"Product {item.ProductId} not found", 400);
            if (product.StockQuantity < item.Quantity)
                throw new AppException($"Insufficient stock for {product.Name}", 400);

            product.StockQuantity -= item.Quantity;
            await _notificationService.NotifyWarehouseManagersLowStockAsync(product);


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
        IQueryable<Order> orders = _context.Orders.Where(o => o.CustomerId == userId);

        if (!string.IsNullOrEmpty(query.Status) &&
            Enum.TryParse<OrderStatus>(query.Status, true, out var status))
            orders = orders.Where(o => o.Status == status);

        return await orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.OrderDate)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(o => new OrderListDto
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status.ToString(),
                TotalAmount = o.TotalAmount,
                Items = o.OrderItems.Select(oi => new OrderItemViewDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<List<OrderListDto>> GetCreatedOrdersAsync(string userId, OrderQueryDto query)
    {
        IQueryable<Order> orders = _context.Orders.Where(o => o.CreatedByUserId == userId);

        if (!string.IsNullOrEmpty(query.Status) &&
            Enum.TryParse<OrderStatus>(query.Status, true, out var status))
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
                TotalAmount = o.TotalAmount,
                Items = o.OrderItems.Select(oi => new OrderItemViewDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task UpdateOrderAsync(int orderId, string userId, UpdateOrderDto dto)
    {
        using var tx = await _context.Database.BeginTransactionAsync();

        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new AppException("Order not found", 404);

        if (order.Status >= OrderStatus.Shipped)
            throw new AppException("Only orders before shipment can be edited", 400);

        if (order.CreatedByUserId != userId && order.CustomerId != userId)
            throw new AppException("You can only edit your own orders", 403);

        if (dto.Items != null)
        {
            var incoming = dto.Items
                .GroupBy(i => i.ProductId)
                .Select(g => new { ProductId = g.Key, Quantity = g.Sum(x => x.Quantity) })
                .ToList();

            var toRemove = order.OrderItems
                .Where(oi => !incoming.Any(i => i.ProductId == oi.ProductId))
                .ToList();

            foreach (var rem in toRemove)
            {
                var p = await _context.Products.FindAsync(rem.ProductId);
                if (p != null) p.StockQuantity += rem.Quantity;
                order.OrderItems.Remove(rem);
            }

            foreach (var item in incoming)
            {
                var product = await _context.Products.FindAsync(item.ProductId)
                    ?? throw new AppException($"Product {item.ProductId} not found", 400);

                var existing = order.OrderItems.FirstOrDefault(x => x.ProductId == item.ProductId);

                if (existing != null)
                {
                    var diff = item.Quantity - existing.Quantity;
                    if (product.StockQuantity < diff)
                        throw new AppException($"Insufficient stock for {product.Name}", 400);

                    product.StockQuantity -= diff;
                    existing.Quantity = item.Quantity;
                    existing.UnitPrice = product.UnitPrice;
                    await _notificationService.NotifyWarehouseManagersLowStockAsync(product);

                }
                else
                {
                    if (product.StockQuantity < item.Quantity)
                        throw new AppException($"Insufficient stock for {product.Name}", 400);

                    product.StockQuantity -= item.Quantity;
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.UnitPrice
                    });
                }
            }

            order.TotalAmount = order.OrderItems.Sum(i => i.UnitPrice * i.Quantity);
        }

        if (dto.PaymentMode.HasValue)
        {
            order.PaymentMode = dto.PaymentMode.Value;
            order.PaymentStatus = dto.PaymentMode == PaymentMode.PayNow
                ? PaymentStatus.Paid
                : PaymentStatus.Pending;
        }

        await _context.SaveChangesAsync();
        await tx.CommitAsync();
    }


    public async Task CancelOrderAsync(int orderId, string userId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new AppException("Order not found", 404);

        if (order.Status is OrderStatus.Shipped or OrderStatus.Delivered)
            throw new AppException("Order cannot be cancelled after shipping", 400);

        foreach (var item in order.OrderItems)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null) product.StockQuantity += item.Quantity;
        }

        order.Status = OrderStatus.Cancelled;
        order.PaymentStatus = order.PaymentMode == PaymentMode.PayNow
            ? PaymentStatus.Refunded
            : PaymentStatus.Unpaid;

        await _context.SaveChangesAsync();
    }

    private static bool IsValidTransition(OrderStatus current, OrderStatus next) =>
        current switch
        {
            OrderStatus.Created => next == OrderStatus.Approved,
            OrderStatus.Approved => next == OrderStatus.Packed,
            OrderStatus.Packed => next == OrderStatus.Shipped,
            OrderStatus.Shipped => next == OrderStatus.Delivered,
            _ => false
        };

    public async Task UpdateOrderStatusAsync(int orderId, string warehouseUserId, OrderStatus newStatus)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new AppException("Order not found", 404);

        var oldStatus = order.Status;

        if (!IsValidTransition(order.Status, newStatus))
            throw new AppException($"Invalid transition {order.Status} → {newStatus}", 400);

        if (newStatus == OrderStatus.Shipped)
        {
            if (order.PaymentStatus != PaymentStatus.Paid)
                throw new AppException("Order cannot be shipped until payment is completed", 400);

            var invoiceExists = await _context.Invoices.AnyAsync(i => i.OrderId == order.Id);
            if (!invoiceExists)
            {
                var invoice = new Invoice
                {
                    OrderId = order.Id,
                    PaymentStatus = order.PaymentStatus,
                    TotalAmount = order.TotalAmount
                };

                foreach (var item in order.OrderItems)
                {
                    invoice.InvoiceItems.Add(new InvoiceItem
                    {
                        ProductId = item.ProductId,
                        ProductName = item.Product.Name,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    });
                }

                _context.Invoices.Add(invoice);
            }
        }

        order.Status = newStatus;
        await _context.SaveChangesAsync();
        await _notificationService.NotifyOrderStatusChangedAsync(order, oldStatus);
    }
    public async Task<List<OrderListDto>> GetAllOrdersAsync(OrderQueryDto query)
    {
        IQueryable<Order> orders = _context.Orders;

        return await orders
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderListDto
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status.ToString(),
                TotalAmount = o.TotalAmount,
                PaymentStatus = o.PaymentStatus.ToString(), // <-- add this line
                Items = o.OrderItems.Select(oi => new OrderItemViewDto
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.Name,
                    UnitPrice = oi.UnitPrice,
                    Quantity = oi.Quantity
                }).ToList()
            })
            .ToListAsync();
    }


}


