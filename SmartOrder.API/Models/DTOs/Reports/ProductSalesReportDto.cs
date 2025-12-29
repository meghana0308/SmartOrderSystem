namespace SmartOrder.API.Models.DTOs.Reports;

public class ProductSalesReportDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}
