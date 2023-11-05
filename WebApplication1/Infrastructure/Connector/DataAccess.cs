using System.Data;
using System.Data.SqlClient;
using WebApplication1.Infrastructure.ConnectionFacory;

namespace WebApplication1.Infrastructure.Connector
{
    #region Generic Persistane

    public class DataAccessLayer : IDataAccess
    {
        protected readonly IConnectionFactory connectionFactory;

        public DataAccessLayer(IConnectionFactory _connectionFactory)
        {
            connectionFactory = _connectionFactory;
        }

        public bool ExecuteData(SqlCommand cmd)
        {
            using (var connection = connectionFactory.GetConnection())
            {
                try
                {
                    cmd.Connection = connection;
                    connection.Open();
                    int result = cmd.ExecuteNonQuery();
                    connection.Close();
                    return result > 0;
                }
                catch (Exception)
                {
                    connection.Close();
                    return false;
                }

            }
        }

        public DataSet GetData(string query)
        {
            using (var connection = connectionFactory.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter(query, connection);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
        }
    }
    #endregion
}
