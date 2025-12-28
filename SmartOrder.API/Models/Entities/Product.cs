namespace SmartOrder.API.Models.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal UnitPrice { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int StockQuantity { get; set; }
    public int ReorderLevel { get; set; }


    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
