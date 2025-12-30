using SmartOrder.API.Models.DTOs.Invoices;

namespace SmartOrder.API.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<List<InvoiceListDto>> GetInvoicesAsync(string? paymentStatus);
        Task<InvoiceDetailDto> GetInvoiceByOrderIdAsync(int orderId);
    }

}
