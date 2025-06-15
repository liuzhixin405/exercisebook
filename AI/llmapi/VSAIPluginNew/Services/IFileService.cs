using System.Collections.Generic;
using System.Threading.Tasks;

namespace VSAIPluginNew.Services
{
    public interface IFileService
    {
        Task<string> ReadFromFileAsync(string path);
        Task<bool> WriteToFileAsync(string path, string content);
        Task LoadSolutionFilesAsync();
        Task<Dictionary<string, string>> GetRelevantFilesAsync(string query);
    }
}