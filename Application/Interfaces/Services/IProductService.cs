using EasyMart.API.Application.Common;
using EasyMart.API.Application.DTOs.Product;

namespace EasyMart.API.Application.Interfaces.Services
{
    public interface IProductService
    {
        public Task<Result<ProductAddResponse>> AddProduct(ProductAddRequest product);
        public Task<Result<IEnumerable<ProductResponse>>> GetAllProducts();
    }
}
