namespace SmartOrder.API.Models.DTOs.Orders;

public class UpdateOrderDto
{
    public List<UpdateOrderItemDto> Items { get; set; } = new();
}
