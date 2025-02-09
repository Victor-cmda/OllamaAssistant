using System;
using System.Text.Json.Serialization;

namespace Core.Entities
{
    public class OllamaModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("size")]
        public long RawSize { get; set; }

        [JsonPropertyName("modified_at")]
        public DateTime Modified { get; set; }

        [JsonPropertyName("digest")]
        public string Digest { get; set; }

        [JsonPropertyName("details")]
        public ModelDetails Details { get; set; }

        public string FormattedSize => FormatSize(RawSize);
        private string FormatSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }
    };
}
