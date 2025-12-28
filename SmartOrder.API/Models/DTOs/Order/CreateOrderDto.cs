using System.ComponentModel.DataAnnotations;

namespace SmartOrder.API.Models.DTOs.Orders;

public class CreateOrderDto
{
    // Required ONLY when Sales creates order
    public string? CustomerId { get; set; }

    [Required]
    public List<CreateOrderItemDto> Items { get; set; } = new();
}
