namespace SmartOrder.API.Models.DTOs.Orders;

public class OrderItemViewDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
