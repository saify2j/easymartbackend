using Dapper;
using EasyMart.API.Application.Interfaces;
using EasyMart.API.Domain.Entities;

public sealed class ProductRepository
    : DapperRepository<Product>, IProductRepository
{
    protected override string TableName => "Products";

    public ProductRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory) { }

    public override async Task<Product?> GetByIdAsync(Guid id)
    {
        const string sql = """
            SELECT ProductId, ProductName, Price
            FROM Products
            WHERE ProductId = @ProductId
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Product>(
            sql, new { ProductId = id });
    }

    public override async Task<int> InsertAsync(Product product)
    {
        const string sql = """
            INSERT INTO Products (ProductId, ProductName, Price)
            VALUES (@ProductId, @ProductName, @Price)
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(sql, product);
    }

    public override async Task<int> UpdateAsync(Product product)
    {
        const string sql = """
            UPDATE Products
            SET ProductName = @ProductName,
                Price = @Price
            WHERE ProductId = @ProductId
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(sql, product);
    }

    public override async Task<int> DeleteAsync(Guid id)
    {
        const string sql = """
            DELETE FROM Products
            WHERE ProductId = @ProductId
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteAsync(sql, new { ProductId = id });
    }

    public async Task<Product?> GetProductByProductName(string productName)
    {
        const string sql = """
            SELECT ProductId, ProductName, Price
            FROM Products
            WHERE ProductName = @productName
            """;

        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Product>(
            sql, new { ProductName = productName });
    }
}
