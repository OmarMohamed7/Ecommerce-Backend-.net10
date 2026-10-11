using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DTOs;

public sealed record UpdateProductRequest(
    [StringLength(100, MinimumLength = 2)]
    string? Name = null,

    [StringLength(500)]
    string? Description = null,

    [Range(typeof(decimal), "0.01", "999999999")]
    decimal? Price = null,

    [Range(0, int.MaxValue)]
    int? Stock = null
);