namespace SmartOrder.API.Models.DTOs.Inventory;

public class InventoryProductDto
{
    public int ProductId { get; set; }
    public string Name { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public int StockQuantity { get; set; }
    public int ReorderLevel { get; set; }
}
