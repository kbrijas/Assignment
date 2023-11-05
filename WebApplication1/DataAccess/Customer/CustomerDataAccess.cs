using System.Data.SqlClient;
using System.Data;
using WebApplication1.Infrastructure.Connector;

namespace WebApplication1.Data.Customer
{
    public class CustomerDataAccess : ICustomerDataAccess
    {
        private readonly IDataAccess _dataAccess;
        public CustomerDataAccess(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public bool AddCustomerData(string Name, string Address)
        {
            SqlCommand cmd = new SqlCommand("INSERT INTO Customers (Name, Address) VALUES (@Name, @Address)");
            cmd.Parameters.AddWithValue("@Name", Name);
            cmd.Parameters.AddWithValue("@Address", Address);

            bool inserted = _dataAccess.ExecuteData(cmd);
            return inserted;
        }

        public DataSet GetCustomerData()
        {
            string query = "SELECT * FROM Customers";
            return _dataAccess.GetData(query);
        }

        public bool UpdateCustomerData(int Id, string Name, string Address)
        {
            // Similar implementation to AddCustomerData
            return true;
        }

    }
}
