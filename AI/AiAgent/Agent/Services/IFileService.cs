using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AiAgent.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string? relativePath = null);
        Task<string> ReadFileContentAsync(string fileName);
        Task<FileExplorerItem> ListCachedFilesAsync(string? path = null);
        Task<bool> DeleteFileAsync(string fileName);
        Task<bool> DeleteAllFilesAsync(string? path = null);
        Task<bool> CreateFolderAsync(string folderName);
        Task SaveFileContentAsync(string path, string content);
    }
} 