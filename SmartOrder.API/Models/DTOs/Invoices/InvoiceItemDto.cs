namespace SmartOrder.API.Models.DTOs.Invoices
{
    public class InvoiceItemDto
    {
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }

}
