namespace SmartOrder.API.Models.DTOs.Product;

public class UpdateProductDto
{
    public required string Name { get; set; }
    public decimal UnitPrice { get; set; }
    public int? CategoryId { get; set; }
}
