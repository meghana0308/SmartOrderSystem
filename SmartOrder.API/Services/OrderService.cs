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

        string customerId;
        if (isSales)
        {
            if (string.IsNullOrEmpty(dto.CustomerId))
                throw new ArgumentException("CustomerId is required for Sales orders");
            customerId = dto.CustomerId;
        }
        else
        {
            customerId = userId; // Customer placing order
        }

        var customerExists = await _context.Users.AnyAsync(u => u.Id == customerId);
        if (!customerExists)
            throw new ArgumentException("Invalid CustomerId");

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
        var orders = _context.Orders.Where(o => o.CustomerId == userId);

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
                TotalAmount = o.TotalAmount
            })
            .ToListAsync();
    }

    public async Task<List<OrderListDto>> GetCreatedOrdersAsync(string userId, OrderQueryDto query)
    {
        var orders = _context.Orders.Where(o => o.CreatedByUserId == userId);

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

        if (order.Status >= OrderStatus.Shipped)
            throw new InvalidOperationException("Only orders before shipment can be edited");

        if (order.CreatedByUserId != userId && order.CustomerId != userId)
            throw new UnauthorizedAccessException("You can only edit your own orders");

        decimal total = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity);

        if (dto.Items != null)
        {
            foreach (var dtoItem in dto.Items)
            {
                if (dtoItem.Quantity <= 0) continue;

                var existingItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == dtoItem.ProductId);
                if (existingItem != null)
                {
                    var product = await _context.Products.FindAsync(dtoItem.ProductId);
                    int diff = dtoItem.Quantity - existingItem.Quantity;
                    if (product.StockQuantity < diff)
                        throw new InvalidOperationException($"Insufficient stock for {product.Name}");
                    product.StockQuantity -= diff;
                    existingItem.Quantity = dtoItem.Quantity;
                    existingItem.UnitPrice = product.UnitPrice;
                }
                else
                {
                    var product = await _context.Products.FindAsync(dtoItem.ProductId);
                    if (product == null)
                        throw new KeyNotFoundException($"Product {dtoItem.ProductId} not found");
                    if (product.StockQuantity < dtoItem.Quantity)
                        throw new InvalidOperationException($"Insufficient stock for {product.Name}");
                    product.StockQuantity -= dtoItem.Quantity;
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = dtoItem.Quantity,
                        UnitPrice = product.UnitPrice
                    });
                }
            }

            total = order.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity);
            order.TotalAmount = total;
        }

        if (dto.PaymentMode.HasValue)
        {
            order.PaymentMode = dto.PaymentMode.Value;
            order.PaymentStatus = dto.PaymentMode == PaymentMode.PayNow
                ? PaymentStatus.Paid
                : PaymentStatus.Pending;
        }

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

        if (order.Status is OrderStatus.Shipped or OrderStatus.Delivered)
            throw new InvalidOperationException("Order cannot be cancelled after shipping");

        foreach (var item in order.OrderItems)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null)
                product.StockQuantity += item.Quantity;
        }

        order.Status = OrderStatus.Cancelled;
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
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            throw new KeyNotFoundException("Order not found");

        if (!IsValidTransition(order.Status, newStatus))
            throw new InvalidOperationException($"Invalid transition {order.Status} → {newStatus}");

        if (newStatus == OrderStatus.Shipped)
        {
            if (order.PaymentStatus != PaymentStatus.Paid)
                throw new InvalidOperationException("Order cannot be shipped until payment is completed");

            var invoiceExists = await _context.Invoices
                .AnyAsync(i => i.OrderId == order.Id);

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
    }
}
