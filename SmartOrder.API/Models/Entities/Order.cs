using SmartOrder.API.Enums;



namespace SmartOrder.API.Models.Entities;

public class Order
{
    public int Id { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    // Customer who owns the order
    public string CustomerId { get; set; } = null!;
    public ApplicationUser Customer { get; set; } = null!;

    // Who created it (Customer or Sales)
    public string CreatedByUserId { get; set; } = null!;
    public ApplicationUser CreatedByUser { get; set; } = null!;

    public OrderStatus Status { get; set; } = OrderStatus.Created;

    public decimal TotalAmount { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public Invoice? Invoice { get; set; }
}
