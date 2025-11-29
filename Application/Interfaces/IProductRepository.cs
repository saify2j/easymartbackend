using EasyMart.API.Domain.Entities;

namespace EasyMart.API.Application.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        public Task<Product?> GetProductByProductName(string productName);
    }
}
