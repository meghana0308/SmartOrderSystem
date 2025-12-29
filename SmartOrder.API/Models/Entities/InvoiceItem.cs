using System.ComponentModel.DataAnnotations.Schema;

namespace SmartOrder.API.Models.Entities
{
    public class InvoiceItem
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = null!;

        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        [NotMapped]
        public decimal LineTotal => Quantity * UnitPrice;
    }
}
