using SmartOrder.API.Models.Entities;
using System.Text.Json.Serialization;

namespace SmartOrder.API.Models.DTOs.Orders;

public class UpdateOrderDto
{
    // Optional: update only if provided
    public List<UpdateOrderItemDto>? Items { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PaymentMode? PaymentMode { get; set; }
}
