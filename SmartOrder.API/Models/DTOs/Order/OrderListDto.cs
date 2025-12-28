namespace SmartOrder.API.Models.DTOs.Orders;

public class OrderListDto
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = null!;
    public decimal TotalAmount { get; set; }
}
