using EasyMart.API.Application.Common;
using EasyMart.API.Application.DTOs.Product;
using EasyMart.API.Application.Interfaces;
using EasyMart.API.Application.Interfaces.Services;
using EasyMart.API.Application.Mappings;
using System.Linq;
using static EasyMart.API.Application.Common.Constants;

namespace EasyMart.API.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUser _currentUser;
        public ProductService(IProductRepository productRepository, ICurrentUser currentUser)
        {
            _productRepository = productRepository;
            _currentUser = currentUser;
        }
        public async Task<Result<ProductAddResponse>> AddProduct(ProductAddRequest request)
        {
            try
            {
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
    }
}
