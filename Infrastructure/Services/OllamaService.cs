using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Windows.Media.Core;
using System.Diagnostics;
using System.IO;

namespace Infrastructure.Services
{
    public class OllamaService : IOllamaService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:11434";

        public OllamaService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromHours(1)
            };
        }

        public async Task<OllamaStatus> CheckOllamaStatus()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/version");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return new OllamaStatus
                    {
                        IsAvailable = true,
                        Version = content,
                        Error = null
                    };
                }

                return new OllamaStatus
                {
                    IsAvailable = false,
                    Version = null,
                    Error = $"Status Code: {response.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new OllamaStatus
                {
                    IsAvailable = false,
                    Version = null,
                    Error = ex.Message
                };
            }
        }

        public async Task<List<OllamaModel>> GetModels()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/tags");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var modelResponse = JsonSerializer.Deserialize<OllamaModelResponse>(content);

                    return modelResponse?.Models ?? new List<OllamaModel>();
                }

                return new List<OllamaModel>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao obter modelos: {ex}");
                return new List<OllamaModel>();
            }
        }

        public async Task<bool> SelectModel(string modelName)
        {
            try
            {
                // Implementar
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveModel(string modelName)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/delete?name={modelName}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<PullResult> PullModel(string modelName, IProgress<PullProgress> progress)
        {
            try
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(new { model = modelName }),
                    Encoding.UTF8,
                    "application/json");

                PullProgress lastProgress = null;

                using var response = await _httpClient.PostAsync("/api/pull", content).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new PullResult(false, errorMessage: $"HTTP {(int)response.StatusCode}: {errorContent}");
                }

                using var stream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(stream);


                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrEmpty(line)) continue;

                    try
                    {
                        var status = JsonSerializer.Deserialize<PullStatus>(line);
                        if (status != null)
                        {
                            lastProgress = new PullProgress
                            {
                                Status = status.Status,
                                Completed = status.Completed,
                                Total = status.Total,
                                DownloadedSize = FormatFileSize(status.Completed),
                                TotalSize = FormatFileSize(status.Total),
                                ProgressPercentage = status.Total > 0
                                    ? (int)((double)status.Completed / status.Total * 100)
                                    : 0
                            };

                            progress?.Report(lastProgress);
                        }
                    }
                    catch (JsonException ex)
                    {
                        Debug.WriteLine($"Erro ao processar JSON: {ex.Message}\nLinha: {line}");
                        continue;
                    }
                }

                return new PullResult(true, lastProgress);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao baixar modelo: {ex.Message}");
                return new PullResult(false);
            }
        }

        public async Task<List<string>> GetAvailableModelsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/tags");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var models = JsonSerializer.Deserialize<TagsResponse>(content);
                    return models.Models.Select(m => m.Name).ToList();
                }
                return new List<string>();
            }
            catch
            {
                return new List<string>
            {
                "llama2",
                "llama2:7b",
                "llama2:13b",
                "llama2:70b",
                "codellama",
                "codellama:7b",
                "codellama:13b",
                "mistral",
                "mistral:7b",
                "neural-chat",
                "orca-mini",
                "vicuna"
            };
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double size = bytes;

            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size = size / 1024;
            }

            return $"{size:0.##} {sizes[order]}";
        }
    }
}
