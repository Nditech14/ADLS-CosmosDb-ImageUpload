using Cosmos.Application.Services;
using Cosmos.Application.Entities; 
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosmos.Core.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Cosmos;

namespace Cosmos.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }


        [HttpGet("GetProductById/{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound($"Product with id {id} not found.");

            return Ok(new { message = product });
        }


        [HttpPost("AddProduct")]
        public async Task<ActionResult<ProductDto>> AddProduct([FromBody] ProductCreationDto product)
        {
            var domainProduct = _mapper.Map<Product>(product);
            if (domainProduct == null)
                return BadRequest("Product data is required.");

            await _productService.AddProductAsync(domainProduct);
            var addedProduct = await _productService.GetProductByIdAsync(domainProduct.id);

            return CreatedAtAction(nameof(GetProductById), new { id = product }, new { message = addedProduct });
        }


        [HttpPut("AddImagesToProduct/{id}")]
        public async Task<ActionResult> AddImagesToProduct(string id, [FromForm] List<IFormFile> images)
        {
            await _productService.UpdateProductImagesAsync(id, images);

            return Ok(new { Message = "Product images added successfully" });
        }


        [HttpPut("UpdateProduct/{id}")]
        public async Task<ActionResult> UpdateProduct(string id, [FromBody] UpdateProductDto updateProductDto)
        {
            if (updateProductDto == null)
                return BadRequest("Product data is required.");

            
            var existingProduct = await _productService.GetProductByIdAsync(id);
            if (existingProduct == null)
                return NotFound($"Product with id {id} not found.");

            
            await _productService.UpdateProductAsync(id, updateProductDto);

       
            var latestupdatedProduct = await _productService.GetProductByIdAsync(id);

            
            return Ok(new
            {
                Product = latestupdatedProduct,
                Message = "Product updated successfully"
            });
        }


        [HttpDelete("DeleteProduct/{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(string id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound($"Product with id {id} not found.");

            await _productService.DeleteProductAsync(id);
            return Ok( new { message = product }); 
        }


        //[HttpGet("GetProducts-loadMore")]
        //public async Task<ActionResult> GetProductsPaged([FromQuery] string continuationToken = null)
        //{
        //    var result = await _productService.GetProductsAsync(continuationToken);

        //    if (result.Products == null || !result.Products.Any())
        //    {
        //        return NotFound("No products found.");
        //    }

        //    var response = new
        //    {
        //        Products = result.Products,
        //        ContinuationToken = result.ContinuationToken
        //    };

        //    return Ok(response);
        //}


        //[HttpGet("all-products")]
        //public async Task<IActionResult> GetAllProducts()
        //{
        //    var result = await _productService.GetAllProductsAsync();

        //    if (result.IsSuccessful) return Ok(result);

        //    return StatusCode(result.Code, result);
        //}

        [HttpGet("load-all-products")]
        public async Task<IActionResult> GetProducts([FromQuery] string continuationToken = null)
        {
            var pageSize = 30; // Set the page size or get it from the query
            var response = await _productService.GetProductsAsync(continuationToken, pageSize);

            if (response.IsSuccessful)
            {
                // Return the list of products along with the continuation token for loading more
                return Ok(new
                {
                    Products = response.Data.Products,
                    ContinuationToken = response.Data.ContinuationToken
                });
            }

            return StatusCode(response.Code, response);
        }
    }
}
