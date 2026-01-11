using EasyMart.API.Application.Common;
using EasyMart.API.Application.DTOs.Product;
using EasyMart.API.Application.Interfaces;
using EasyMart.API.Application.Interfaces.Services;
using EasyMart.API.Application.Mappings;
using FluentValidation;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static EasyMart.API.Application.Common.Constants;
using SixLabors.ImageSharp;

namespace EasyMart.API.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IValidator<ProductAddRequest> _productValidator;

        public ProductService(IProductRepository productRepository, ICurrentUser currentUser, IValidator<ProductAddRequest> productAddValidator)
        {
            _productRepository = productRepository;
            _currentUser = currentUser;
            _productValidator = productAddValidator;
        }
        public async Task<Result<ProductAddResponse>> AddProduct(ProductAddRequest request)
        {
            try
            {
                var validationResult = await _productValidator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    var errorMessage = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));

                    return Result<ProductAddResponse>.Failure(
                    ResponseCodes.Error,
                    errorMessage
                   );
                }

                var product = request.ToDomain();

                // get username from current user
                product.CreatedBy = _currentUser.Username ?? "";

                var existing_product = await _productRepository.GetProductByProductName(product.ProductName);

                if (existing_product != null)
                {
                    throw new Exception(CustomMessages.ProductAlreadyExists);
                }
                await _productRepository.InsertAsync(product);

                return Result<ProductAddResponse>.Success(
                    ResponseCodes.Success,
                    CustomMessages.ProductCreated,
                    product.ToAddResponse());

            }
            catch (Exception ex)
            {
                return Result<ProductAddResponse>.Failure
                    (ResponseCodes.Error, ex.Message.ToString());
            }
        }

        public async Task<Result<IEnumerable<ProductResponse>>> GetAllProducts()
        {
            var products = await _productRepository.GetAllAsync();
            var response = products.Select(p => p.ToResponse());

            return Result<IEnumerable<ProductResponse>>.Success(
                ResponseCodes.Success,
                CustomMessages.Success,
                response);
        }


        public async Task<byte[]> ConvertToWebPLosslessAsync(Stream input)
            {
                using var image = await Image.LoadAsync<Rgba32>(input);

                var encoder = new WebpEncoder
                {
                    FileFormat = WebpFileFormatType.Lossy,
                    Quality = 80,
                    Method = WebpEncodingMethod.Fastest
                };

                using var output = new MemoryStream();
                await image.SaveAsync(output, encoder);
                return output.ToArray();
            }

        }
}
