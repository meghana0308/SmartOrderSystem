using SmartOrder.API.Enums;

namespace SmartOrder.API.Models.DTOs.Orders;

public class UpdateOrderStatusDto
{
    public OrderStatus Status { get; set; }
}
