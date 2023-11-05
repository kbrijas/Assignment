using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using WebApplication1.Models;

namespace WebApplication1.Infrastructure.ConnectionFacory
{

    public class ConnectionFactory : IConnectionFactory
    {
        public readonly ApplicationOptions _appConfig;
        private string _connectionString = string.Empty;

        public ConnectionFactory(IOptions<ApplicationOptions> appConfig)
        {
            _appConfig = appConfig.Value ?? throw new ArgumentNullException(nameof(appConfig)); ;
            _connectionString = _appConfig.ConnectionString;
        }

        public SqlConnection GetConnection()
        {
            var connection = new SqlConnection(_connectionString);
            return connection;
        }
    }
}
