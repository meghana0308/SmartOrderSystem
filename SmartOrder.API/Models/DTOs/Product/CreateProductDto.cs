namespace SmartOrder.API.Models.DTOs.Product;

public class CreateProductDto
{
    public required string Name { get; set; }
    public decimal UnitPrice { get; set; }
    public int CategoryId { get; set; }
    public int StockQuantity { get; set; }
    public int ReorderLevel { get; set; }

}
