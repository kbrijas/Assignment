namespace WebApplication1.Middlewares.Storage
{
    public interface IStorage
    {
        Task<string> GetAsync(string id);
        Task DeleteAsync(string id);
        Task SetAsync(string id, string data);
    }


}
