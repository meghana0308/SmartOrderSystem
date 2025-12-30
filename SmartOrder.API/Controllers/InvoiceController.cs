using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartOrder.API.Data;
using SmartOrder.API.Enums;
using SmartOrder.API.Models.DTOs.Invoices;

namespace SmartOrder.API.Controllers;

[ApiController]
[Route("api/invoices")]
[Authorize(Roles = "FinanceOfficer")]
public class InvoiceController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public InvoiceController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var invoices = await _context.Invoices
            .AsNoTracking()
            .Select(i => new InvoiceListDto
            {
                InvoiceId = i.Id,
                OrderId = i.OrderId,
                InvoiceDate = i.InvoiceDate,
                TotalAmount = i.TotalAmount,
                PaymentStatus = i.PaymentStatus.ToString()

            })
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync();

        return Ok(invoices);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var invoice = await _context.Invoices
            .AsNoTracking()
            .Include(i => i.InvoiceItems)
            .Where(i => i.Id == id)
            .Select(i => new InvoiceDetailDto
            {
                InvoiceId = i.Id,
                OrderId = i.OrderId,
                InvoiceDate = i.InvoiceDate,
                TotalAmount = i.TotalAmount,
                PaymentStatus = i.PaymentStatus.ToString(),
                Items = i.InvoiceItems.Select(ii => new InvoiceItemDto
                {
                    ProductName = ii.ProductName,
                    Quantity = ii.Quantity,
                    UnitPrice = ii.UnitPrice,
                    LineTotal = ii.Quantity * ii.UnitPrice
                }).ToList()
            })
            .FirstOrDefaultAsync();

        if (invoice == null)
            return NotFound();

        return Ok(invoice);
    }

    [HttpGet("order/{orderId:int}")]
    public async Task<IActionResult> GetByOrder(int orderId)
    {
        var invoice = await _context.Invoices
            .AsNoTracking()
            .Where(i => i.OrderId == orderId)
            .Select(i => new InvoiceListDto
            {
                InvoiceId = i.Id,
                OrderId = i.OrderId,
                InvoiceDate = i.InvoiceDate,
                TotalAmount = i.TotalAmount,
                PaymentStatus = i.PaymentStatus.ToString()

            })
            .FirstOrDefaultAsync();

        if (invoice == null)
            return NotFound();

        return Ok(invoice);
    }

    [HttpGet("finance/orders")]
    public async Task<IActionResult> GetAllOrdersForFinance(
    [FromQuery] PaymentStatus? paymentStatus)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Include(o => o.Invoice)
            .AsQueryable();

        if (paymentStatus.HasValue)
            query = query.Where(o => o.PaymentStatus == paymentStatus);

        var result = await query
            .Select(o => new FinanceOrderDto
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate,
                OrderStatus = o.Status.ToString(),
                PaymentMode = o.PaymentMode,
                PaymentStatus = o.PaymentStatus.ToString(),
                TotalAmount = o.TotalAmount,
                InvoiceId = o.Invoice != null ? o.Invoice.Id : null
            })
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return Ok(result);
    }

}
