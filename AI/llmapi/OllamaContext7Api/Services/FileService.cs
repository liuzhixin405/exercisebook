using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OllamaContext7Api.Services
{
    // 新增：用于表示文件或文件夹的结构
    public class FileExplorerItem
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty; // 相对路径或完整路径
        public string Type { get; set; } = string.Empty; // "file" 或 "directory"
        public List<FileExplorerItem>? Children { get; set; }
    }

    public class FileService : IFileService
    {
        private readonly string _fileCacheDirectory;
        private readonly ILogger<FileService> _logger;

        public FileService(ILogger<FileService> logger, IOptions<FileServiceOptions> options)
        {
            _logger = logger;
            _fileCacheDirectory = options.Value.FileCacheDirectory;

            if (string.IsNullOrEmpty(_fileCacheDirectory))
            {
                throw new ArgumentNullException("FileCacheDirectory", "File cache directory is not configured in appsettings.");
            }
            
            if (!Directory.Exists(_fileCacheDirectory))
            {
                Directory.CreateDirectory(_fileCacheDirectory);
                _logger.LogInformation($"创建文件缓存目录: {_fileCacheDirectory}");
            }
            else
            {
                _logger.LogInformation($"文件缓存目录已存在: {_fileCacheDirectory}");
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file, string? relativePath = null)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("文件无效。");
            }

            // 如果提供了相对路径，则使用它构建完整的目标路径，否则使用原始文件名
            string targetPath;
            string fileNameToReturn;

            if (!string.IsNullOrEmpty(relativePath))
            {
                // 结合文件缓存目录和相对路径来创建目标目录
                var fullDirectoryPath = Path.Combine(_fileCacheDirectory, Path.GetDirectoryName(relativePath) ?? string.Empty);
                if (!Directory.Exists(fullDirectoryPath))
                {
                    Directory.CreateDirectory(fullDirectoryPath);
                }
                targetPath = Path.Combine(_fileCacheDirectory, relativePath);
                fileNameToReturn = relativePath;
            }
            else
            {
                targetPath = Path.Combine(_fileCacheDirectory, Path.GetFileName(file.FileName));
                fileNameToReturn = Path.GetFileName(file.FileName);
            }

            using (var stream = new FileStream(targetPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation($"文件 '{fileNameToReturn}' 已保存到缓存。");
            return fileNameToReturn;
        }

        public async Task SaveFileContentAsync(string path, string content)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("文件路径不能为空。", nameof(path));
            }

            var fullPath = Path.Combine(_fileCacheDirectory, path);
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            _logger.LogInformation($"正在保存文件内容到: {path}");
            await File.WriteAllTextAsync(fullPath, content);
            _logger.LogInformation($"文件内容已成功保存到: {path}");
        }

        public async Task<string> ReadFileContentAsync(string fileName)
        {
            // 调整以处理包含路径的 fileName
            var filePath = Path.Combine(_fileCacheDirectory, fileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"文件 '{fileName}' 不存在于缓存中。");
            }

            _logger.LogInformation($"正在读取文件 '{fileName}' 的内容。");
            return await File.ReadAllTextAsync(filePath);
        }

        public Task<FileExplorerItem> ListCachedFilesAsync(string? path = null)
        {
            _logger.LogInformation($"正在列出缓存文件和文件夹，路径: {path ?? "根目录"}。");
            var targetDirectory = _fileCacheDirectory;
            string currentDirectoryName = "根目录";

            if (!string.IsNullOrEmpty(path))
            {
                targetDirectory = Path.Combine(_fileCacheDirectory, path);
                currentDirectoryName = Path.GetFileName(targetDirectory);

                if (!Directory.Exists(targetDirectory))
                {
                    _logger.LogWarning($"指定路径 '{path}' 不存在，将返回空目录结构。");
                    return Task.FromResult(new FileExplorerItem
                    {
                        Name = currentDirectoryName,
                        Path = path,
                        Type = "directory",
                        Children = new List<FileExplorerItem>()
                    });
                }
            }
            
            // 构建当前目录的 FileExplorerItem，并将其子项设置为 BuildFileTree 的结果
            var currentDirectoryItem = new FileExplorerItem
            {
                Name = currentDirectoryName,
                Path = path ?? string.Empty,
                Type = "directory",
                Children = BuildFileTree(targetDirectory, _fileCacheDirectory) // 获取子项列表
            };

            _logger.LogInformation($"已列出 {currentDirectoryItem.Children.Count} 个顶层文件/文件夹于路径: {path ?? "根目录"}。");
            return Task.FromResult(currentDirectoryItem);
        }

        private List<FileExplorerItem> BuildFileTree(string currentDirectory, string rootDirectory)
        {
            var items = new List<FileExplorerItem>();

            // 添加子目录
            foreach (var dirPath in Directory.GetDirectories(currentDirectory))
            {
                var dirInfo = new DirectoryInfo(dirPath);
                var relativePath = Path.GetRelativePath(rootDirectory, dirPath);
                items.Add(new FileExplorerItem
                {
                    Name = dirInfo.Name,
                    Path = relativePath,
                    Type = "directory",
                    Children = BuildFileTree(dirPath, rootDirectory) // 递归构建子目录
                });
            }

            // 添加文件
            foreach (var filePath in Directory.GetFiles(currentDirectory))
            {
                var fileInfo = new FileInfo(filePath);
                var relativePath = Path.GetRelativePath(rootDirectory, filePath);
                items.Add(new FileExplorerItem
                {
                    Name = fileInfo.Name,
                    Path = relativePath,
                    Type = "file"
                });
            }

            return items.OrderBy(item => item.Name).ToList();
        }

        public Task<bool> DeleteFileAsync(string fileName)
        {
            // 调整以处理包含路径的 fileName
            var filePath = Path.Combine(_fileCacheDirectory, fileName);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation($"文件 '{fileName}' 已从缓存中删除。");
                return Task.FromResult(true);
            }
            // 如果是目录，则尝试删除目录
            else if (Directory.Exists(filePath))
            {
                Directory.Delete(filePath, recursive: true);
                _logger.LogInformation($"文件夹 '{fileName}' 已从缓存中删除。");
                return Task.FromResult(true);
            }
            _logger.LogWarning($"尝试删除文件/文件夹 '{fileName}'，但它不存在。");
            return Task.FromResult(false);
        }

        public Task<bool> DeleteAllFilesAsync(string? path = null)
        {
            var targetDirectory = _fileCacheDirectory;
            if (!string.IsNullOrEmpty(path))
            {
                targetDirectory = Path.Combine(_fileCacheDirectory, path);
                if (!Directory.Exists(targetDirectory))
                {
                    _logger.LogWarning($"尝试删除路径 '{path}' 中的所有文件，但该路径不存在。");
                    return Task.FromResult(false);
                }
            }

            _logger.LogInformation($"正在删除路径 '{path ?? "根目录"}' 中的所有缓存文件和文件夹...");
            try
            {
                // 删除目标目录中的所有文件
                foreach (var file in Directory.GetFiles(targetDirectory))
                {
                    File.Delete(file);
                }
                // 删除目标目录中的所有子目录及其内容
                foreach (var dir in Directory.GetDirectories(targetDirectory))
                {
                    Directory.Delete(dir, recursive: true);
                }
                _logger.LogInformation($"路径 '{path ?? "根目录"}' 中的所有缓存文件和文件夹已成功删除。");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除路径 '{path ?? "根目录"}' 中的所有文件失败。");
                return Task.FromResult(false);
            }
        }

        public Task<bool> CreateFolderAsync(string folderName)
        {
            _logger.LogInformation($"正在创建文件夹: {folderName}...");
            try
            {
                var folderPath = Path.Combine(_fileCacheDirectory, folderName);
                if (Directory.Exists(folderPath))
                {
                    _logger.LogWarning($"文件夹 '{folderName}' 已经存在。");
                    return Task.FromResult(false);
                }
                Directory.CreateDirectory(folderPath);
                _logger.LogInformation($"文件夹 '{folderName}' 已成功创建。");
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"创建文件夹 '{folderName}' 失败。");
                return Task.FromResult(false);
            }
        }
    }
} 