using EasyMart.API.Application.DTOs.Product;
using EasyMart.API.Domain.Entities;

namespace EasyMart.API.Application.Mappings;

public static class ProductMappingExtensions
{
    public static Product ToDomain(this ProductAddRequest request)
    {
        return new Product
        {
            ProductId = Guid.NewGuid(),
            ProductName = request.ProductName,
            Price = request.Price
        };
    }

    public static ProductAddResponse ToAddResponse(this Product product)
    {
        return new ProductAddResponse
        {
            ProductId = product.ProductId.ToString(),
            ProductName = product.ProductName,
            Price = product.Price
        };
    }
    public static ProductResponse ToResponse(this Product product)
    {
        return new ProductResponse
        {
            ProductId = product.ProductId.ToString(),
            ProductName = product.ProductName,
            Price = product.Price
        };
    }
}
