using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Core.Entities
{
    public class TagsResponse
    {
        [JsonPropertyName("models")]
        public List<ModelInfo> Models { get; set; }
    }
}
