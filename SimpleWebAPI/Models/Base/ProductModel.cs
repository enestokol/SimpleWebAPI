using System.ComponentModel.DataAnnotations;

namespace SimpleWebAPI.Models.Base;

public class ProductModel
{
    public int Id { get; set; }
    [Required] public string Name { get; set; } = null!;
    [Required] public decimal Price { get; set; }
}