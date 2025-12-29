using System.ComponentModel.DataAnnotations;
using SmartOrder.API.Models.Entities;

namespace SmartOrder.API.Models.DTOs.Orders;

public class CreateOrderDto
{
    // Required ONLY when Sales creates order
    public string? CustomerId { get; set; }

    [Required]
    public List<CreateOrderItemDto> Items { get; set; } = new();

    public PaymentMode PaymentMode { get; set; } = PaymentMode.PayLater;
}
