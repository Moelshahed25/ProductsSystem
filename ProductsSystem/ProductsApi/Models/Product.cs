using System.ComponentModel.DataAnnotations;

namespace ProductsApi.Models;

public class Product
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
