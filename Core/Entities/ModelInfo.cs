using System;
using System.Text.Json.Serialization;

namespace Core.Entities
{
    public class ModelInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }

        [JsonPropertyName("digest")]
        public string Digest { get; set; }

        [JsonPropertyName("modified_at")]
        public DateTime ModifiedAt { get; set; }
    }
}
