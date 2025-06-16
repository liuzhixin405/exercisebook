using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VSAIPluginNew.Services
{
    public interface IAIService
    {
        Task<string> GenerateReplyAsync(string prompt, CancellationToken cancellationToken = default);
        Task<string> ProcessQueryAsync(
            string query, 
            string taskType, 
            Dictionary<string, string>? contextFiles = null, 
            CancellationToken cancellationToken = default);
    }    public interface IFileService
    {
        Task<bool> WriteToFileAsync(string path, string content);
        Task<string> ReadFromFileAsync(string path);
        Task LoadSolutionFilesAsync();
        Dictionary<string, string> GetOpenedFiles();
        Task<Dictionary<string, string>> GetAllSolutionFilesContentAsync();
        Task<Dictionary<string, string>> GetRelevantFilesAsync(string query);
    }
}
