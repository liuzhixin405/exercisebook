using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

namespace VSAIPluginNew.Services
{
    public class MCPService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly string _baseDirectory;

        public MCPService(string? baseDirectory = null)
        {
            _baseDirectory = baseDirectory ?? Directory.GetCurrentDirectory();
        }

        #region Filesystem操作

        // 读取文件内容
        public async Task<string> ReadFileAsync(string filePath)
        {
            try
            {
                // 确保路径是相对于基础目录的
                string fullPath = GetFullPath(filePath);
                
                if (!File.Exists(fullPath))
                {
                    return $"错误: 文件不存在 - {filePath}";
                }

                return await Task.Run(() => File.ReadAllText(fullPath));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"读取文件错误: {ex.Message}");
                return $"读取文件时出错: {ex.Message}";
            }
        }

        // 写入文件
        public async Task<bool> WriteFileAsync(string filePath, string content)
        {
            try
            {
                string fullPath = GetFullPath(filePath);
                
                // 确保目录存在
                string? directory = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    await Task.Run(() => Directory.CreateDirectory(directory));
                }

                await Task.Run(() => File.WriteAllText(fullPath, content));
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"写入文件错误: {ex.Message}");
                return false;
            }
        }

        // 创建目录
        public async Task<bool> CreateDirectoryAsync(string dirPath)
        {
            try
            {
                string fullPath = GetFullPath(dirPath);
                if (!Directory.Exists(fullPath))
                {
                    await Task.Run(() => Directory.CreateDirectory(fullPath));
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"创建目录错误: {ex.Message}");
                return false;
            }
        }

        // 列出目录内容
        public async Task<List<string>> ListDirectoryAsync(string dirPath)
        {
            try
            {
                string fullPath = GetFullPath(dirPath);
                
                if (!Directory.Exists(fullPath))
                {
                    return new List<string>() { $"错误: 目录不存在 - {dirPath}" };
                }

                var result = new List<string>();
                await Task.Run(() =>
                {
                    var files = Directory.GetFiles(fullPath)
                                       .Select(f => Path.GetFileName(f));
                    var directories = Directory.GetDirectories(fullPath)
                                             .Select(d => Path.GetFileName(d) + "/");
                    result.AddRange(directories.Concat(files));
                });
                
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"列出目录错误: {ex.Message}");
                return new List<string>() { $"列出目录时出错: {ex.Message}" };
            }
        }

        // 搜索文件
        public async Task<List<string>> SearchFilesAsync(string basePath, string pattern, string[]? excludePatterns = null)
        {
            if (string.IsNullOrEmpty(basePath))
            {
                basePath = _baseDirectory;
            }

            if (string.IsNullOrEmpty(pattern))
            {
                pattern = "*.*";
            }

            try
            {
                string fullBasePath = GetFullPath(basePath);
                
                if (!Directory.Exists(fullBasePath))
                {
                    return new List<string>() { $"错误: 目录不存在 - {basePath}" };
                }

                var result = new List<string>();
                await Task.Run(() =>
                {
                    var files = Directory.GetFiles(fullBasePath, pattern, SearchOption.AllDirectories);
                    
                    if (excludePatterns != null && excludePatterns.Length > 0)
                    {
                        files = files.Where(f => !excludePatterns.Any(p => 
                            f.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0)).ToArray();
                    }
                    
                    // 转换为相对路径
                    result.AddRange(files.Select(f => GetRelativePath(fullBasePath, f)));
                });
                
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"搜索文件错误: {ex.Message}");
                return new List<string>() { $"搜索文件时出错: {ex.Message}" };
            }
        }

        // 获取完整路径
        private string GetFullPath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }
            return Path.GetFullPath(Path.Combine(_baseDirectory, path));
        }

        // 获取相对路径的兼容实现
        private string GetRelativePath(string relativeTo, string path)
        {
            var relativeToUri = new Uri(relativeTo.EndsWith("\\") ? relativeTo : relativeTo + "\\");
            var pathUri = new Uri(path);
            var relativeUri = relativeToUri.MakeRelativeUri(pathUri);
            return Uri.UnescapeDataString(relativeUri.ToString().Replace('/', '\\'));
        }

        #endregion

        #region Fetch操作

        // 从URL获取内容
        public async Task<string> FetchUrlAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"获取URL内容错误: {ex.Message}");
                return $"获取URL内容时出错: {ex.Message}";
            }
        }

        // 执行POST请求
        public async Task<string> PostDataAsync(string url, string jsonData)
        {
            try
            {
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"POST请求错误: {ex.Message}");
                return $"执行POST请求时出错: {ex.Message}";
            }
        }

        #endregion

        #region Sequential Thinking操作

        // 进行顺序思考
        public async Task<string> SequentialThinkingAsync(string problem, int steps = 5)
        {
            try
            {
                StringBuilder result = new StringBuilder();
                result.AppendLine("# 顺序思考过程");
                result.AppendLine();

                await Task.Run(() => 
                {
                    // 模拟思考步骤
                    for (int i = 1; i <= steps; i++)
                    {
                        result.AppendLine($"## 步骤 {i}");
                        result.AppendLine();
                        
                        if (i == 1)
                        {
                            result.AppendLine("首先，我需要理解问题的要求:");
                            result.AppendLine(problem);
                        }
                        else if (i == steps)
                        {
                            result.AppendLine("综合前面的分析，得出结论:");
                        }
                        else
                        {
                            result.AppendLine($"考虑第{i}部分的分析:");
                        }
                        
                        result.AppendLine();
                    }
                });

                return result.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"顺序思考错误: {ex.Message}");
                return $"执行顺序思考时出错: {ex.Message}";
            }
        }

        #endregion

        #region Tool操作

        // 执行工具调用
        public async Task<string> ExecuteToolAsync(string toolName, string args)
        {
            try
            {
                switch (toolName.ToLower())
                {
                    case "读取文件":
                        return await ReadFileAsync(args);

                    case "写入文件":
                        {
                            // 分割参数为路径和内容
                            int spaceIndex = args.IndexOf(' ');
                            if (spaceIndex <= 0)
                            {
                                return "错误: 写入文件需要提供路径和内容，格式：路径 内容";
                            }
                            string path = args.Substring(0, spaceIndex);
                            string content = args.Substring(spaceIndex + 1);
                            bool success = await WriteFileAsync(path, content);
                            return success ? "文件写入成功" : "文件写入失败";
                        }

                    case "创建目录":
                        {
                            bool success = await CreateDirectoryAsync(args);
                            return success ? "目录创建成功" : "目录创建失败";
                        }

                    case "列出目录":
                        {
                            var files = await ListDirectoryAsync(args);
                            if (files == null || files.Count == 0)
                                return "目录为空或不存在";
                            return string.Join("\n", files);
                        }

                    case "搜索文件":
                        {
                            string pattern;
                            string searchDir;

                            int spaceIndex = args.IndexOf(' ');
                            if (spaceIndex > 0)
                            {
                                pattern = args.Substring(0, spaceIndex);
                                searchDir = args.Substring(spaceIndex + 1);
                            }
                            else
                            {
                                pattern = args;
                                searchDir = _baseDirectory;
                            }

                            var files = await SearchFilesAsync(pattern, searchDir);
                            return string.Join("\n", files);
                        }

                    case "获取网页":
                        return await FetchUrlAsync(args);

                    case "发送请求":
                        {
                            int spaceIndex = args.IndexOf(' ');
                            if (spaceIndex <= 0)
                            {
                                return "错误: 发送请求需要提供URL和JSON内容，格式：URL JSON内容";
                            }
                            string url = args.Substring(0, spaceIndex);
                            string jsonData = args.Substring(spaceIndex + 1);
                            return await PostDataAsync(url, jsonData);
                        }

                    case "顺序思考":
                        {
                            // 处理可选的步数参数
                            string problem;
                            int steps = 5; // 默认5步
                            
                            int spaceIndex = args.LastIndexOf(' ');
                            if (spaceIndex > 0)
                            {
                                // 尝试解析最后一个部分为步数
                                string lastPart = args.Substring(spaceIndex + 1).Trim();
                                if (int.TryParse(lastPart, out int parsedSteps))
                                {
                                    problem = args.Substring(0, spaceIndex).Trim();
                                    steps = parsedSteps;
                                }
                                else
                                {
                                    problem = args;
                                }
                            }
                            else
                            {
                                problem = args;
                            }

                            return await SequentialThinkingAsync(problem, steps);
                        }
                        
                    default:
                        return $"错误: 不支持的工具 '{toolName}'";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"工具执行错误: {ex.Message}");
                return $"工具 '{toolName}' 执行时出错: {ex.Message}";
            }
        }

        #endregion
    }
}