using System.Data.SqlClient;

namespace WebApplication1.Infrastructure.ConnectionFacory
{
    public interface IConnectionFactory
    {
        SqlConnection GetConnection();
    }
}
