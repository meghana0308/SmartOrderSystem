using Microsoft.EntityFrameworkCore;
using SmartOrder.API.Data;
using SmartOrder.API.Enums;
using SmartOrder.API.Models.DTOs.Invoices;
using SmartOrder.API.Services.Interfaces;

namespace SmartOrder.API.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _context;

        public InvoiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<InvoiceListDto>> GetInvoicesAsync(string? paymentStatus)
        {
            var query = _context.Invoices.AsNoTracking();

            if (!string.IsNullOrEmpty(paymentStatus) &&
                Enum.TryParse<PaymentStatus>(paymentStatus, true, out var status))
            {
                query = query.Where(i => i.PaymentStatus == status);
            }

            return await query
                .Select(i => new InvoiceListDto
                {
                    InvoiceId = i.Id,
                    OrderId = i.OrderId,
                    InvoiceDate = i.InvoiceDate,
                    TotalAmount = i.TotalAmount,
                    PaymentStatus = i.PaymentStatus.ToString()
                })
                .ToListAsync();
        }

        public async Task<InvoiceDetailDto> GetInvoiceByOrderIdAsync(int orderId)
        {
            var invoice = await _context.Invoices
                .AsNoTracking()
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.OrderId == orderId);

            if (invoice == null)
                throw new KeyNotFoundException("Invoice not found");

            return new InvoiceDetailDto
            {
                InvoiceId = invoice.Id,
                OrderId = invoice.OrderId,
                InvoiceDate = invoice.InvoiceDate,
                TotalAmount = invoice.TotalAmount,
                PaymentStatus = invoice.PaymentStatus.ToString(),
                Items = invoice.InvoiceItems.Select(ii => new InvoiceItemDto
                {
                    ProductName = ii.ProductName,
                    Quantity = ii.Quantity,
                    UnitPrice = ii.UnitPrice,
                    LineTotal = ii.Quantity * ii.UnitPrice
                }).ToList()
            };
        }
    }

}
