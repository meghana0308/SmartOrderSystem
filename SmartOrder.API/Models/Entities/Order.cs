using SmartOrder.API.Enums;
using System.Text.Json.Serialization;

namespace SmartOrder.API.Models.Entities;

public enum PaymentMode
{
    PayNow,
    PayLater
}

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public string CustomerId { get; set; } = null!;
    public ApplicationUser Customer { get; set; } = null!;

    public string CreatedByUserId { get; set; } = null!;
    public ApplicationUser CreatedByUser { get; set; } = null!;

    public OrderStatus Status { get; set; } = OrderStatus.Created;

    public decimal TotalAmount { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public Invoice? Invoice { get; set; }

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PaymentMode PaymentMode { get; set; } = PaymentMode.PayLater;
}
