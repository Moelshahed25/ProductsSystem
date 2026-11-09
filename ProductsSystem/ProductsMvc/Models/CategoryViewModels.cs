using System.ComponentModel.DataAnnotations;

namespace ProductsMvc.Models;

public class CategoryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProductsCount { get; set; }
}

public class CategoryFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required"), StringLength(100)]
    public string Name { get; set; } = string.Empty;
}
