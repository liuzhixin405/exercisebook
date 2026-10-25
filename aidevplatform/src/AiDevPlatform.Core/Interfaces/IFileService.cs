namespace AiDevPlatform.Core.Interfaces;

public interface IFileService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder = "uploads");
    Task<string> SaveImageAsync(Stream imageStream, string fileName, string folder = "images");
    Task<Stream> GetFileAsync(string filePath);
    Task<bool> DeleteFileAsync(string filePath);
    Task<bool> FileExistsAsync(string filePath);
    Task<long> GetFileSizeAsync(string filePath);
    Task<string> GetFileMimeTypeAsync(string fileName);
    Task<bool> IsValidImageAsync(Stream imageStream);
    Task<string> GenerateUniqueFileNameAsync(string originalFileName);
    Task<List<string>> GetFilesInDirectoryAsync(string directory);
    Task<bool> CreateDirectoryAsync(string directory);
    Task<bool> DeleteDirectoryAsync(string directory);
}
