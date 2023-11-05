namespace WebApplication1.Data.Customer
{
    public interface ICustomerDataAccess
    {
        public bool AddCustomerData(string Name, string Address);
        public bool UpdateCustomerData(int Id, string Name, string Address);
    }
}
