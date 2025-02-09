using System.Text.Json.Serialization;

namespace Core.Entities
{
    public class PullStatus
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("completed")]
        public long Completed { get; set; }

        [JsonPropertyName("total")]
        public long Total { get; set; }

        [JsonPropertyName("digest")]
        public string Digest { get; set; }

        [JsonPropertyName("total_size")]
        public long TotalSize { get; set; }
    }

}
