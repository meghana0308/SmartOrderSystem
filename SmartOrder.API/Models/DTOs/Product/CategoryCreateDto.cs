using System.ComponentModel.DataAnnotations;

namespace SmartOrder.API.Models.DTOs.Product;

public class CategoryCreateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
}
