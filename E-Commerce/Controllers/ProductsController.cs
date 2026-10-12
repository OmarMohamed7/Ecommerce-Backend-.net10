using E_Commerce.DTOs;
using E_Commerce.Models;
using E_Commerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(IProductService productService) : ControllerBase
{
        [HttpGet]
        public IActionResult GetProducts() => Ok(productService.GetAll());

        [HttpGet("{id:int}")]
        public IActionResult GetProduct(int id)
        {
                var product = productService.GetById(id);
                return product is null ? NotFound() : Ok(product);
        }

        [HttpPost]
        public IActionResult PostProduct([FromBody] CreateProductRequest productDto)
        {
                var product = new Product
                {
                        Id = new Random().Next(1, int.MaxValue),
                        Name = productDto.Name,
                        Price = productDto.Price,
                        Stock = productDto.Stock,
                        Description = productDto.Description,

                };
                
                var res = productService.Create(product);
                
                return CreatedAtAction("PostProduct", new { id = res.Id }, product);
        }

        [HttpPatch("{id:int}")]
        public IActionResult UpdateProduct(
                int id,
                [FromBody] UpdateProductRequest request)
        {
                
                var res = productService.Update(id, request);
                
                if(res is null) return NotFound();
                
                return Ok(res);
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteProduct(int id)
        {
                var product = productService.GetById(id);
                if (product is null)
                {
                        return NotFound();
                }

                productService.Delete(id);
                
                return NoContent();
        }
}
