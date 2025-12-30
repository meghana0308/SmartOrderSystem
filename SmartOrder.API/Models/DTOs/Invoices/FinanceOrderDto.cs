using SmartOrder.API.Models.Entities;

public class FinanceOrderDto
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string OrderStatus { get; set; } = null!;
    public PaymentMode PaymentMode { get; set; }
    public string PaymentStatus { get; set; } = null!;
    public decimal TotalAmount { get; set; }

    public int? InvoiceId { get; set; }
}
