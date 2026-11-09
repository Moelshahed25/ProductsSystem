using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProductsMvc.Models;

public class ProductViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}

public class ProductFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required"), StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Price is required")]
    [Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "Price must be greater than 0")]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "Please select a category")]
    [Display(Name = "Category")]
    public int? CategoryId { get; set; }

    public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
}
