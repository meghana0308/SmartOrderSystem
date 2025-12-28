namespace SmartOrder.API.Models.DTOs.Product;

public class ProductResponseDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal UnitPrice { get; set; }
    public required string CategoryName { get; set; }
}
