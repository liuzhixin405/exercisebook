using Microsoft.Extensions.Logging;
using AiDevPlatform.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AiDevPlatform.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileService> _logger;

    public FileService(IConfiguration configuration, ILogger<FileService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder = "uploads")
    {
        try
        {
            var uploadPath = _configuration["FileUpload:UploadPath"] ?? "uploads";
            var fullPath = Path.Combine(uploadPath, folder);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            var uniqueFileName = await GenerateUniqueFileNameAsync(fileName);
            var filePath = Path.Combine(fullPath, uniqueFileName);

            using var fileStreamOutput = new FileStream(filePath, FileMode.Create);
            await fileStream.CopyToAsync(fileStreamOutput);

            _logger.LogInformation("File saved successfully: {FilePath}", filePath);
            return filePath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save file: {FileName}", fileName);
            throw;
        }
    }

    public async Task<string> SaveImageAsync(Stream imageStream, string fileName, string folder = "images")
    {
        try
        {
            if (!await IsValidImageAsync(imageStream))
            {
                throw new ArgumentException("Invalid image file");
            }

            imageStream.Position = 0; // Reset stream position
            return await SaveFileAsync(imageStream, fileName, folder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save image: {FileName}", fileName);
            throw;
        }
    }

    public async Task<Stream> GetFileAsync(string filePath)
    {
        try
        {
            if (!await FileExistsAsync(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get file: {FilePath}", filePath);
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        try
        {
            if (await FileExistsAsync(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file: {FilePath}", filePath);
            return false;
        }
    }

    public async Task<bool> FileExistsAsync(string filePath)
    {
        return await Task.FromResult(File.Exists(filePath));
    }

    public async Task<long> GetFileSizeAsync(string filePath)
    {
        try
        {
            if (await FileExistsAsync(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                return fileInfo.Length;
            }
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get file size: {FilePath}", filePath);
            return 0;
        }
    }

    public async Task<string> GetFileMimeTypeAsync(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".txt" => "text/plain",
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".json" => "application/json",
            ".xml" => "application/xml",
            _ => "application/octet-stream"
        };
    }

    public async Task<bool> IsValidImageAsync(Stream imageStream)
    {
        try
        {
            // Check if stream is valid
            if (imageStream == null || imageStream.Length == 0)
                return false;

            // Read first few bytes to check image signature
            var buffer = new byte[8];
            var originalPosition = imageStream.Position;
            imageStream.Position = 0;
            await imageStream.ReadAsync(buffer, 0, 8);
            imageStream.Position = originalPosition;

            // Check for common image signatures
            return IsImageSignature(buffer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate image");
            return false;
        }
    }

    public async Task<string> GenerateUniqueFileNameAsync(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = Guid.NewGuid().ToString("N")[..8];

        return await Task.FromResult($"{nameWithoutExtension}_{timestamp}_{random}{extension}");
    }

    public async Task<List<string>> GetFilesInDirectoryAsync(string directory)
    {
        try
        {
            if (!Directory.Exists(directory))
            {
                return new List<string>();
            }

            var files = Directory.GetFiles(directory, "*", SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileName)
                .Where(name => name != null)
                .Cast<string>()
                .ToList();

            return await Task.FromResult(files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get files in directory: {Directory}", directory);
            return new List<string>();
        }
    }

    public async Task<bool> CreateDirectoryAsync(string directory)
    {
        try
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                _logger.LogInformation("Directory created: {Directory}", directory);
            }
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create directory: {Directory}", directory);
            return false;
        }
    }

    public async Task<bool> DeleteDirectoryAsync(string directory)
    {
        try
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
                _logger.LogInformation("Directory deleted: {Directory}", directory);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete directory: {Directory}", directory);
            return false;
        }
    }

    private static bool IsImageSignature(byte[] buffer)
    {
        if (buffer.Length < 4)
            return false;

        // JPEG
        if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
            return true;

        // PNG
        if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47)
            return true;

        // GIF
        if (buffer[0] == 0x47 && buffer[1] == 0x49 && buffer[2] == 0x46)
            return true;

        // BMP
        if (buffer[0] == 0x42 && buffer[1] == 0x4D)
            return true;

        // WebP
        if (buffer.Length >= 12 &&
            buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46 &&
            buffer[8] == 0x57 && buffer[9] == 0x45 && buffer[10] == 0x42 && buffer[11] == 0x50)
            return true;

        return false;
    }
}
