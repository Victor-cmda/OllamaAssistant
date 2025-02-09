using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Core.Entities
{
    public class OllamaModelResponse
    {
        [JsonPropertyName("models")]
        public List<OllamaModel> Models { get; set; }
    }

}
