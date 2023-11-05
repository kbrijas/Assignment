using System.Data.SqlClient;
using System.Data;

namespace WebApplication1.Infrastructure.Connector
{
    public interface IDataAccess
    {
        public bool ExecuteData(SqlCommand cmd);
        public DataSet GetData(string query);
    }
}
