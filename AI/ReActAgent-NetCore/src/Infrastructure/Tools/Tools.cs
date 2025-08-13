using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ReActAgentNetCore.Infrastructure.ProjectCreation;
using ReActAgentNetCore.Infrastructure.MCP;

namespace ReActAgentNetCore.Infrastructure.Tools
{
    /// <summary>
    /// 工具类 - 提供文件、目录、终端等操作功能
    /// </summary>
    public static class Tools
    {
        // 存储项目目录的静态字段
        private static string _projectDirectory = Directory.GetCurrentDirectory();
        
        // 设置项目目录的方法
        public static void SetProjectDirectory(string projectDirectory)
        {
            _projectDirectory = projectDirectory;
        }
        
        // 获取项目目录的方法
        public static string GetProjectDirectory()
        {
            return _projectDirectory;
        }

        // ========== 文件操作工具 ==========

        /// <summary>
        /// 读取文件内容
        /// </summary>
        public static async Task<string> ReadFileAsync(string filePath)
        {
            try
            {
                // 将相对路径解析为相对于项目目录的绝对路径
                var absolutePath = Path.IsPathRooted(filePath) ? filePath : Path.Combine(GetProjectDirectory(), filePath);
                
                if (!File.Exists(absolutePath))
                {
                    return $"文件不存在：{absolutePath}";
                }

                // 检查文件大小（限制为1MB）
                var fileInfo = new FileInfo(absolutePath);
                if (fileInfo.Length > 1024 * 1024)
                {
                    return $"文件过大（{fileInfo.Length} bytes），请使用其他方式查看大文件";
                }

                var content = await File.ReadAllTextAsync(absolutePath, System.Text.Encoding.UTF8);
                return content;
            }
            catch (UnauthorizedAccessException)
            {
                return $"无权限访问文件：{filePath}";
            }
            catch (Exception ex)
            {
                return $"读取文件失败：{ex.Message}";
            }
        }

        /// <summary>
        /// 将指定内容写入指定文件
        /// </summary>
        public static async Task<string> WriteToFileAsync(string filePath, string content)
        {
            try
            {
                // 将相对路径解析为相对于项目目录的绝对路径
                var absolutePath = Path.IsPathRooted(filePath) ? filePath : Path.Combine(GetProjectDirectory(), filePath);
                
                var directory = Path.GetDirectoryName(absolutePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 处理转义字符
                content = content.Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\r", "\r");

                await File.WriteAllTextAsync(absolutePath, content, System.Text.Encoding.UTF8);
                return $"写入成功：{absolutePath}";
            }
            catch (UnauthorizedAccessException)
            {
                return $"无权限写入文件：{filePath}";
            }
            catch (Exception ex)
            {
                return $"写入文件失败：{ex.Message}";
            }
        }

        // ========== 目录操作工具 ==========

        /// <summary>
        /// 列出指定目录的内容
        /// </summary>
        public static string ListDirectoryAsync(string directoryPath)
        {
            try
            {
                var absolutePath = Path.IsPathRooted(directoryPath) ? directoryPath : Path.Combine(GetProjectDirectory(), directoryPath);
                
                if (!Directory.Exists(absolutePath))
                {
                    return $"目录不存在：{absolutePath}";
                }

                var items = Directory.GetFileSystemEntries(absolutePath);
                var result = new System.Text.StringBuilder();
                result.AppendLine($"📁 目录内容：{absolutePath}");
                result.AppendLine(new string('-', 50));

                foreach (var item in items)
                {
                    var name = Path.GetFileName(item);
                    var isDirectory = Directory.Exists(item);
                    var icon = isDirectory ? "📁" : "📄";
                    result.AppendLine($"{icon} {name}");
                }

                return result.ToString();
            }
            catch (UnauthorizedAccessException)
            {
                return $"无权限访问目录：{directoryPath}";
            }
            catch (Exception ex)
            {
                return $"列出目录失败：{ex.Message}";
            }
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        public static string CreateDirectoryAsync(string directoryPath)
        {
            try
            {
                var absolutePath = Path.IsPathRooted(directoryPath) ? directoryPath : Path.Combine(GetProjectDirectory(), directoryPath);
                
                if (Directory.Exists(absolutePath))
                {
                    return $"目录已存在：{absolutePath}";
                }

                Directory.CreateDirectory(absolutePath);
                return $"目录创建成功：{absolutePath}";
            }
            catch (UnauthorizedAccessException)
            {
                return $"无权限创建目录：{directoryPath}";
            }
            catch (Exception ex)
            {
                return $"创建目录失败：{ex.Message}";
            }
        }

        /// <summary>
        /// 搜索文件
        /// </summary>
        public static string SearchFilesAsync(string searchPattern, string directoryPath = "")
        {
            try
            {
                var searchDir = string.IsNullOrEmpty(directoryPath) ? GetProjectDirectory() : 
                    (Path.IsPathRooted(directoryPath) ? directoryPath : Path.Combine(GetProjectDirectory(), directoryPath));

                if (!Directory.Exists(searchDir))
                {
                    return $"搜索目录不存在：{searchDir}";
                }

                var files = Directory.GetFiles(searchDir, searchPattern, SearchOption.AllDirectories);
                var result = new System.Text.StringBuilder();
                result.AppendLine($"🔍 搜索结果：在 {searchDir} 中搜索 {searchPattern}");
                result.AppendLine(new string('-', 50));

                if (files.Length == 0)
                {
                    result.AppendLine("未找到匹配的文件");
                }
                else
                {
                    foreach (var file in files)
                    {
                        var relativePath = Path.GetRelativePath(searchDir, file);
                        result.AppendLine($"📄 {relativePath}");
                    }
                    result.AppendLine($"\n共找到 {files.Length} 个文件");
                }

                return result.ToString();
            }
            catch (UnauthorizedAccessException)
            {
                return $"无权限搜索目录：{directoryPath}";
            }
            catch (Exception ex)
            {
                return $"搜索文件失败：{ex.Message}";
            }
        }

        // ========== 终端命令工具 ==========

        /// <summary>
        /// 执行终端命令
        /// </summary>
        public static async Task<string> RunTerminalCommandAsync(string command)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = Environment.OSVersion.Platform == PlatformID.Win32NT ? "cmd.exe" : "/bin/bash",
                        Arguments = Environment.OSVersion.Platform == PlatformID.Win32NT ? $"/c {command}" : $"-c \"{command}\"",
                        WorkingDirectory = GetProjectDirectory(),
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                var output = new System.Text.StringBuilder();
                var error = new System.Text.StringBuilder();

                process.OutputDataReceived += (sender, e) => 
                {
                    if (e.Data != null) output.AppendLine(e.Data);
                };
                process.ErrorDataReceived += (sender, e) => 
                {
                    if (e.Data != null) error.AppendLine(e.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();

                var result = new System.Text.StringBuilder();
                result.AppendLine($"🚀 命令执行完成：{command}");
                result.AppendLine($"工作目录：{GetProjectDirectory()}");
                result.AppendLine($"退出代码：{process.ExitCode}");
                
                if (output.Length > 0)
                {
                    result.AppendLine("\n📤 标准输出：");
                    result.AppendLine(output.ToString());
                }
                
                if (error.Length > 0)
                {
                    result.AppendLine("\n❌ 错误输出：");
                    result.AppendLine(error.ToString());
                }

                return result.ToString();
            }
            catch (Exception ex)
            {
                return $"执行命令失败：{ex.Message}";
            }
        }

        // ========== 智能项目创建工具 ==========

        /// <summary>
        /// 创建.NET项目
        /// </summary>
        public static async Task<string> CreateDotNetProjectAsync(string projectType, string projectName)
        {
            try
            {
                var creator = new CLIProjectCreator();
                var result = await creator.CreateDotNetProjectAsync(projectType, projectName, GetProjectDirectory());
                return result.Success ? result.Message ?? "创建成功" : $"创建失败：{result.ErrorMessage ?? "未知错误"}";
            }
            catch (Exception ex)
            {
                return $"创建.NET项目时发生异常：{ex.Message}";
            }
        }

        /// <summary>
        /// 创建Node.js项目
        /// </summary>
        public static async Task<string> CreateNodeProjectAsync(string projectType, string projectName)
        {
            try
            {
                var creator = new CLIProjectCreator();
                var result = await creator.CreateNodeProjectAsync(projectType, projectName, GetProjectDirectory());
                return result.Success ? result.Message ?? "创建成功" : $"创建失败：{result.ErrorMessage ?? "未知错误"}";
            }
            catch (Exception ex)
            {
                return $"创建Node.js项目时发生异常：{ex.Message}";
            }
        }

        /// <summary>
        /// 创建Python项目
        /// </summary>
        public static async Task<string> CreatePythonProjectAsync(string projectType, string projectName)
        {
            try
            {
                var creator = new CLIProjectCreator();
                var result = await creator.CreatePythonProjectAsync(projectType, projectName, GetProjectDirectory());
                return result.Success ? result.Message ?? "创建成功" : $"创建失败：{result.ErrorMessage ?? "未知错误"}";
            }
            catch (Exception ex)
            {
                return $"创建Python项目时发生异常：{ex.Message}";
            }
        }

        /// <summary>
        /// 创建区块链项目
        /// </summary>
        public static async Task<string> CreateBlockchainProjectAsync(string projectType, string projectName)
        {
            try
            {
                var creator = new CLIProjectCreator();
                var result = await creator.CreateBlockchainProjectAsync(projectType, projectName, GetProjectDirectory());
                return result.Success ? result.Message ?? "创建成功" : $"创建失败：{result.ErrorMessage ?? "未知错误"}";
            }
            catch (Exception ex)
            {
                return $"创建区块链项目时发生异常：{ex.Message}";
            }
        }

        /// <summary>
        /// 创建移动应用项目
        /// </summary>
        public static async Task<string> CreateMobileProjectAsync(string projectType, string projectName)
        {
            try
            {
                var creator = new CLIProjectCreator();
                var result = await creator.CreateMobileProjectAsync(projectType, projectName, GetProjectDirectory());
                return result.Success ? result.Message ?? "创建成功" : $"创建失败：{result.ErrorMessage ?? "未知错误"}";
            }
            catch (Exception ex)
            {
                return $"创建移动应用项目时发生异常：{ex.Message}";
            }
        }

        /// <summary>
        /// 通过MCP创建项目
        /// </summary>
        public static async Task<string> CreateProjectViaMCPAsync(string projectType, string projectName)
        {
            try
            {
                var result = await MCPToolIntegrator.CreateProjectViaMCP(projectType, projectName, GetProjectDirectory());
                return result.Success ? result.Message ?? "创建成功" : $"创建失败：{result.ErrorMessage ?? "未知错误"}";
            }
            catch (Exception ex)
            {
                return $"MCP项目创建时发生异常：{ex.Message}";
            }
        }

        // ========== 工具方法 ==========

        /// <summary>
        /// 格式化文件大小
        /// </summary>
        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }
    }
} 