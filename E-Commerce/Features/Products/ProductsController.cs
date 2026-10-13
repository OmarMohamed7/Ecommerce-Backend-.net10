using AutoMapper;
using E_Commerce.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using E_Commerce.Common;
using E_Commerce.Features.Auth;

namespace E_Commerce.Features.Products;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(
    IProductService productService,
    IMapper mapper,
    IValidator<CreateProductDto> createValidator,
    IValidator<UpdateProductRequest> updateValidator) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] ProductQueryParams queryParams)
    {
        return Ok(ApiResponse<PagedResult<Product>>.Ok(await productService.GetAll(queryParams)));
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await productService.GetById(id);
        return product is null
            ? NotFound(ApiResponse<Product>.Fail("Product not found."))
            : Ok(ApiResponse<Product>.Ok(product));
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<IActionResult> PostProduct([FromBody] CreateProductDto productDto)
    {
        var validationResult = await createValidator.ValidateAsync(productDto);
        if (!validationResult.IsValid) return BadRequest(ApiResponse<Product>.Fail(validationResult.ToApiErrors()));

        var product = mapper.Map<Product>(productDto);

        var createdProduct = await productService.Create(product);

        return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id },
            ApiResponse<Product>.Ok(createdProduct));
    }

    [Authorize(Roles = Roles.Admin)]
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

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await productService.GetById(id);
        if (product is null) return NotFound(ApiResponse<object>.Fail("Product not found."));

        var res = await productService.Delete(id);

        if (res) return NoContent();

        return NotFound(ApiResponse<object>.Fail("Product not found."));
    }

    [HttpGet("test-error")]
    public IActionResult TestError()
    {
        throw new Exception(" Test Error");
    }
}