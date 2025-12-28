namespace SmartOrder.API.Models.DTOs.Orders;

public class UpdateOrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
