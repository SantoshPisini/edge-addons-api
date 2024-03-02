namespace SantoshPisini.EdgeAddonsAPI.Models
{
    public class PublishModel
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AccessTokenURL { get; set; }
        public string ProductId { get; set; }
        public string PackageUrl { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
