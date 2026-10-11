using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTOs;

public sealed record CreateProductRequest(
    [Required]
    [StringLength(100, MinimumLength = 2)]
    string Name,

    [Required]
    [StringLength(500)]
    string Description,

    [Range(typeof(decimal), "0.01", "999999999")]
    decimal Price,

    [Range(0, int.MaxValue)]
    int Stock
);