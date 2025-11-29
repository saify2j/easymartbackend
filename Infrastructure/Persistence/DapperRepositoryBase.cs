using Dapper;
using EasyMart.API.Application.Interfaces;
using System.Data;

public abstract class DapperRepository<T> : IRepository<T>
    where T : class
{
    protected readonly IDbConnectionFactory _connectionFactory;

    protected DapperRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    protected abstract string TableName { get; }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        const string sql = "SELECT * FROM {{TABLE}}";
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<T>(
            sql.Replace("{{TABLE}}", TableName)
        );
    }

    public abstract Task<T?> GetByIdAsync(Guid id);
    public abstract Task<int> InsertAsync(T entity);
    public abstract Task<int> UpdateAsync(T entity);
    public abstract Task<int> DeleteAsync(Guid id);
}
