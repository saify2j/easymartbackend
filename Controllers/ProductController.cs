using EasyMart.API.Application.Common;
using EasyMart.API.Application.Common.Interfaces;
using EasyMart.API.Application.DTOs.Product;
using EasyMart.API.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyMart.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        private readonly IRequestContext _requestContext;

        public ProductController(IProductService productService, ILogger<ProductController> logger, IRequestContext requestContext)
        {
            _productService = productService;
            _logger = logger;
            _requestContext = requestContext;
        }

        [Authorize(Policy = "SuperAdminPolicy")]
        [HttpPost("AddProduct")]
        [ProducesResponseType(typeof(ApiResponse<ProductAddResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddProduct(ProductAddRequest request)
        {
            //_logger.LogInformation("AddProduct request received", _requestContext.RequestId, "126.0.0.1");
            _logger.LogInformation(
                   "AddProduct request received | RequestId={RequestId} | ClientIp={ClientIp}",
                   _requestContext.RequestId,
                   _requestContext.ClientIp);
            var result = await _productService.AddProduct(request);

            if (!result.IsSuccess)
            {
                return Conflict(new ApiResponse<object>(
                    result.Code,
                    result.Message,
                    null));
            }

            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponse<ProductAddResponse>(
                    result.Code,
                    result.Message,
                    result.Value));
        }

        [Authorize(Policy = "Anybody")]
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

        [HttpPost("convert-to-webp")]
        public async Task<IActionResult> ConvertToWebp(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Invalid file");

            await using var inputStream = file.OpenReadStream();
            var webpBytes = await _productService.ConvertToWebPLosslessAsync(inputStream);

            return File(webpBytes, "image/webp",
                Path.ChangeExtension(file.FileName, ".webp"));
        }
    }
}
