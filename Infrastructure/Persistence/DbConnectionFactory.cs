using EasyMart.API.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace EasyMart.API.Infrastructure.Persistence
{
    

    public sealed class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;
        private readonly DatabaseProvider _provider;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default")!;
            _provider = Enum.Parse<DatabaseProvider>(
                configuration["DatabaseProvider"]!, true);
        }

        public IDbConnection CreateConnection()
        {
            return _provider switch
            {
                DatabaseProvider.SqlServer => new SqlConnection(_connectionString),
                DatabaseProvider.PostgreSql => new NpgsqlConnection(_connectionString),
                DatabaseProvider.Oracle => new OracleConnection(_connectionString),
                _ => throw new NotSupportedException("Database provider not supported")
            };
        }
    }
}
