using E_Commerce.DTOs;
using E_Commerce.Extensions;
using E_Commerce.Models;
using E_Commerce.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(
    IProductService productService,
    IValidator<CreateProductDto> createValidator,
    IValidator<UpdateProductRequest> updateValidator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        return Ok(ApiResponse<List<Product>>.Ok(await productService.GetAll()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await productService.GetById(id);
        return product is null
            ? NotFound(ApiResponse<Product>.Fail("Product not found."))
            : Ok(ApiResponse<Product>.Ok(product));
    }

    [HttpPost]
    public async Task<IActionResult> PostProduct([FromBody] CreateProductDto productDto)
    {
        var validationResult = await createValidator.ValidateAsync(productDto);
        if (!validationResult.IsValid) return BadRequest(ApiResponse<Product>.Fail(validationResult.ToApiErrors()));

        var product = new Product
        {
            Name = productDto.Name,
            Price = productDto.Price,
            Stock = productDto.Stock,
            Description = productDto.Description
        };

        var res = await productService.Create(product);

        return CreatedAtAction(nameof(GetProduct), new { id = res.Id }, ApiResponse<Product>.Ok(res));
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateProduct(
        int id,
        [FromBody] UpdateProductRequest request)
    {
        var validationResult = await updateValidator.ValidateAsync(request);
        if (!validationResult.IsValid) return BadRequest(ApiResponse<Product>.Fail(validationResult.ToApiErrors()));

        var res = await productService.Update(id, request);

        if (res is null) return NotFound(ApiResponse<Product>.Fail("Product not found."));

        return Ok(ApiResponse<Product>.Ok(res));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await productService.GetById(id);
        if (product is null) return NotFound(ApiResponse<object>.Fail("Product not found."));

        var res = await productService.Delete(id);

        if (res)
        {
            return NoContent();
        }
        else
        {
            return NotFound(ApiResponse<object>.Fail("Product not found."));
        }
    }
}