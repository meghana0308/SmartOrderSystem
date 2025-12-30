namespace SmartOrder.API.Models.DTOs.Invoices
{
    public class InvoiceListDto
    {
        public int InvoiceId { get; set; }
        public int OrderId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; } = null!;
    }

}
