using EasyMart.API.Application.Common;
using EasyMart.API.Application.DTOs.Product;
using EasyMart.API.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EasyMart.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpPost("AddProduct")]
        [ProducesResponseType(typeof(ApiResponse<ProductAddResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddProduct(ProductAddRequest request)
        {
            var result = await _productService.AddProduct(request);

            if (!result.IsSuccess)
            {
                return Conflict(new ApiResponse<object>(
                    result.Code,
                    result.Message,
                    null));
            }

            _logger.LogInformation(
                "Product added. ProductId={ProductId}",
                result.Value?.ProductId);

            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponse<ProductAddResponse>(
                    result.Code,
                    result.Message,
                    result.Value));
        }


        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _productService.GetAllProducts();

            return Ok(new ApiResponse<IEnumerable<ProductResponse>>(
                result.Code,
                result.Message,
                result.Value));
        }
    }
}
