using System.ComponentModel.DataAnnotations;

namespace SimpleWebAPI.Models.Request;

public class ProductUpdateModel
{
    [Required] public int Id { get; init; }
    [Required(AllowEmptyStrings = false, ErrorMessage = "The field {0} is cannot be null or empty")] public string Name { get; init; }
    [Required] [Range(0, 1_000_000_000_000)] public decimal Price { get; init; }
}