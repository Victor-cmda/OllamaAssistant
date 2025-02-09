using Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IOllamaService
    {
        Task<OllamaStatus> CheckOllamaStatus();
        Task<List<OllamaModel>> GetModels();
        Task<bool> SelectModel(string modelName);
        Task<bool> RemoveModel(string modelName);
        Task<PullResult> PullModel(string modelName, IProgress<PullProgress> progress);
        Task<List<string>> GetAvailableModelsAsync();
    }
}
