using System.Text.Json.Serialization;

namespace SantoshPisini.EdgeAddonsAPI.Models
{
    public class StatusResponseModel
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("createdTime")]
        public string? CreatedTime { get; set; }

        [JsonPropertyName("lastUpdatedTime")]
        public string? LastUpdatedTime { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("errorCode")]
        public string? ErrorCode { get; set; }

        [JsonPropertyName("errors")]
        public List<Error>? Errors { get; set; }
    }

    public class Error
    {
        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
