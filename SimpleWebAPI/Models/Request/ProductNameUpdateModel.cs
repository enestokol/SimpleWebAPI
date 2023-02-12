using System.ComponentModel.DataAnnotations;

namespace SimpleWebAPI.Models.Request;

public class ProductNameUpdateModel
{
    [Required] public int Id { get; init; }
    [Required(AllowEmptyStrings = false, ErrorMessage = "The field {0} is cannot be null or empty")] public string Name { get; init; } = null!;
}