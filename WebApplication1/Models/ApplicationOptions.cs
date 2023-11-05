namespace WebApplication1.Models
{
    public class ApplicationOptions
    {

        public const string Secrets = "Secrets";

        /// <summary>
        /// Property to read Connection String
        /// </summary>
        public string? ConnectionString { get; set; }
    }
}
