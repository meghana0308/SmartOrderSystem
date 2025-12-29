using SmartOrder.API.Enums;

namespace SmartOrder.API.Models.Entities;

public class Invoice
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }

    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}
