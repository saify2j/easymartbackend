using System.Data;

namespace EasyMart.API.Application.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
