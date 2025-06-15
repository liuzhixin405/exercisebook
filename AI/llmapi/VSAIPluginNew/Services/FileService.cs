using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System.Diagnostics;

namespace VSAIPluginNew.Services
{
    public class FileService : IFileService
    {
        private readonly DTE2 _dte;
        private readonly string _baseDirectory;
        private Dictionary<string, string> _openedFiles;

        public FileService(DTE2 dte, string baseDirectory = null)
        {
            _dte = dte ?? throw new ArgumentNullException(nameof(dte));
            _baseDirectory = baseDirectory ?? Directory.GetCurrentDirectory();
            _openedFiles = new Dictionary<string, string>();
        }

        public async Task<string> ReadFromFileAsync(string path)
        {
            try
            {
                string fullPath = GetFullPath(path);
                if (!File.Exists(fullPath))
                {
                    Debug.WriteLine($"文件不存在: {fullPath}");
                    return null;
                }

                return await Task.Run(() => File.ReadAllText(fullPath));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"读取文件错误: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> WriteToFileAsync(string path, string content)
        {
            try
            {
                string fullPath = GetFullPath(path);
                string directory = Path.GetDirectoryName(fullPath);
                
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    await CreateDirectoryAsync(directory);
                }

                await Task.Run(() => File.WriteAllText(fullPath, content));
                await RefreshFileInVSAsync(fullPath);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"写入文件错误: {ex.Message}");
                return false;
            }
        }

        public async Task LoadSolutionFilesAsync()
        {
            try
            {
                _openedFiles = await GetAllSolutionFilesContentAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"加载解决方案文件错误: {ex.Message}");
                _openedFiles = new Dictionary<string, string>();
            }
        }

        public Dictionary<string, string> GetOpenedFiles()
        {
            return _openedFiles;
        }

        public async Task<bool> CreateDirectoryAsync(string directoryPath)
        {
            try
            {
                string fullPath = GetFullPath(directoryPath);
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

        public async Task<List<string>> ListDirectoryAsync(string directoryPath)
        {
            try
            {
                string fullPath = GetFullPath(directoryPath);
                if (!Directory.Exists(fullPath))
                {
                    return new List<string>();
                }

                return await Task.Run(() => Directory.GetFiles(fullPath).ToList());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"列出目录错误: {ex.Message}");
                return new List<string>();
            }
        }

        public async Task<string> GetActiveDocumentContentAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            try
            {
                Document activeDoc = _dte.ActiveDocument;
                if (activeDoc?.Object("TextDocument") is TextDocument textDoc)
                {
                    EditPoint editPoint = textDoc.StartPoint.CreateEditPoint();
                    return editPoint.GetText(textDoc.EndPoint);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"获取活动文档内容错误: {ex.Message}");
            }

            return null;
        }

        public async Task<bool> UpdateActiveDocumentContentAsync(string newContent)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            try
            {
                Document activeDoc = _dte.ActiveDocument;
                if (activeDoc?.Object("TextDocument") is TextDocument textDoc)
                {
                    EditPoint editPoint = textDoc.StartPoint.CreateEditPoint();
                    editPoint.Delete(textDoc.EndPoint);
                    editPoint.Insert(newContent);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"更新活动文档内容错误: {ex.Message}");
            }

            return false;
        }

        public async Task<Dictionary<string, string>> GetAllSolutionFilesContentAsync()
        {
            var files = new Dictionary<string, string>();

            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                // 获取当前打开的文件
                var openFiles = GetOpenedFiles();
                foreach (var file in openFiles)
                {
                    if (!files.ContainsKey(file.Key))
                    {
                        files[file.Key] = file.Value;
                    }
                }

                // 在主线程中获取项目文件列表
                var projectFiles = new List<string>();
                if (_dte.Solution != null && _dte.Solution.Projects != null)
                {
                    foreach (Project project in _dte.Solution.Projects)
                    {
                        await CollectProjectFilesAsync(project.ProjectItems, projectFiles);
                    }
                }

                // 切换到后台线程读取文件内容
                await Task.Delay(1);
                foreach (var filePath in projectFiles)
                {
                    if (!files.ContainsKey(filePath) && IsCodeFile(filePath))
                    {
                        try
                        {
                            string content = await ReadFromFileAsync(filePath);
                            if (!string.IsNullOrEmpty(content))
                            {
                                files[filePath] = content;
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"读取文件 {filePath} 时出错: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"获取文件内容错误: {ex.Message}");
            }

            return files;
        }

        private async Task CollectProjectFilesAsync(ProjectItems items, List<string> files)
        {
            if (items == null) return;

            foreach (ProjectItem item in items)
            {
                try
                {
                    // 确保在主线程访问 ProjectItem
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    if (item.FileCount > 0)
                    {
                        string filePath = item.FileNames[1]; // 索引1是完整路径
                        if (!files.Contains(filePath))
                        {
                            files.Add(filePath);
                        }
                    }

                    if (item.ProjectItems != null)
                    {
                        await CollectProjectFilesAsync(item.ProjectItems, files);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"处理项目项时出错: {ex.Message}");
                }
            }
        }

        private bool IsCodeFile(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            return ext switch
            {
                ".cs" => true,
                ".ts" => true,
                ".js" => true,
                ".html" => true,
                ".css" => true,
                ".json" => true,
                ".xml" => true,
                ".config" => true,
                _ => false
            };
        }
        
       

        public async Task<List<string>> SearchFilesAsync(string pattern, string searchDir = null)
        {
            try
            {
                string searchPath = GetFullPath(searchDir ?? _baseDirectory);
                return Directory.GetFiles(searchPath, pattern, SearchOption.AllDirectories).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"搜索文件错误: {ex.Message}");
                return new List<string>();
            }
        }

        private List<string> GetAllSolutionFiles()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var fileList = new List<string>();
            foreach (Project project in _dte.Solution.Projects.Cast<Project>())
            {
                try
                {
                    if (project?.ProjectItems == null) continue;
                    GetProjectFiles(project.ProjectItems, fileList);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"处理项目错误 {project?.Name}: {ex.Message}");
                }
            }
            return fileList;
        }

        private void GetProjectFiles(ProjectItems items, List<string> fileList)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            foreach (ProjectItem item in items)
            {
                try
                {
                    if (item.FileCount > 0)
                    {
                        string fileName = item.FileNames[0];
                        if (!fileList.Contains(fileName))
                        {
                            fileList.Add(fileName);
                        }
                    }

                    if (item.ProjectItems != null)
                    {
                        GetProjectFiles(item.ProjectItems, fileList);
                    }

                    if (item.SubProject != null)
                    {
                        GetProjectFiles(item.SubProject.ProjectItems, fileList);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"处理项目项错误: {ex.Message}");
                }
            }
        }

        private async Task RefreshFileInVSAsync(string filePath)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            try
            {
                foreach (Document doc in _dte.Documents)
                {
                    if (string.Equals(doc.FullName, filePath, StringComparison.OrdinalIgnoreCase))
                    {
                        doc.Close(vsSaveChanges.vsSaveChangesNo);
                        _dte.ItemOperations.OpenFile(filePath);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"刷新VS中的文件错误: {ex.Message}");
            }
        }

        private string GetFullPath(string path)
        {
            return Path.IsPathRooted(path) ? path : Path.GetFullPath(Path.Combine(_baseDirectory, path));
        }

        private bool IsSupportedCodeFile(string filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            return new[] { ".cs", ".vb", ".ts", ".js", ".html", ".css", ".json", ".xml" }.Contains(ext);
        }

        // 已过时的同步方法，使用异步版本代替
        public void WriteToFile(string path, string content)
        {
            throw new NotImplementedException("请使用WriteToFileAsync方法");
        }

        public string ReadFromFile(string path)
        {
            throw new NotImplementedException("请使用ReadFromFileAsync方法");
        }

        public async Task<Dictionary<string, string>> GetRelevantFilesAsync(string query)
        {
            try
            {
                const int MAX_FILES = 5; // 最大文件数
                const int MAX_FILE_SIZE = 100 * 1024; // 每个文件最大100KB
                var relevantFiles = new Dictionary<string, string>();

                // 1. 首先获取活动文档
                var activeDocContent = await GetActiveDocumentContentAsync();
                if (activeDocContent != null)
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    var activeDoc = _dte.ActiveDocument;
                    if (activeDoc != null)
                    {
                        relevantFiles[activeDoc.FullName] = TruncateContent(activeDocContent, MAX_FILE_SIZE);
                    }
                }

                // 如果查询为空，只返回活动文档
                if (string.IsNullOrWhiteSpace(query))
                {
                    return relevantFiles;
                }

                // 2. 获取所有打开的文件
                var openedFiles = GetOpenedFiles();
                var scoredFiles = new List<(string path, string content, double score)>();

                // 3. 对文件内容进行相关性评分
                foreach (var file in openedFiles)
                {
                    if (relevantFiles.ContainsKey(file.Key)) continue;

                    double score = CalculateRelevanceScore(file.Value, query);
                    if (score > 0)
                    {
                        scoredFiles.Add((file.Key, file.Value, score));
                    }
                }

                // 4. 选择评分最高的文件
                var topFiles = scoredFiles
                    .OrderByDescending(f => f.score)
                    .Take(MAX_FILES - relevantFiles.Count);

                foreach (var file in topFiles)
                {
                    relevantFiles[file.path] = TruncateContent(file.content, MAX_FILE_SIZE);
                }

                return relevantFiles;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"获取相关文件时出错: {ex.Message}");
                return new Dictionary<string, string>();
            }
        }

        private string TruncateContent(string content, int maxSize)
        {
            if (string.IsNullOrEmpty(content) || content.Length <= maxSize)
                return content;

            // 保留文件的开头和结尾部分
            int halfSize = maxSize / 2;
            return content.Substring(0, halfSize) +
                   "\n... (content truncated) ...\n" +
                   content.Substring(content.Length - halfSize);
        }

        private double CalculateRelevanceScore(string content, string query)
        {
            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(query))
                return 0;

            content = content.ToLower();
            query = query.ToLower();

            double score = 0;
            
            // 1. 检查完整查询匹配
            if (content.Contains(query))
            {
                score += 10;
            }

            // 2. 检查查询中的关键词匹配
            var keywords = query.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var keyword in keywords)
            {
                if (keyword.Length < 3) continue; // 忽略太短的词
                if (content.Contains(keyword))
                {
                    score += 1;
                }
            }

            // 3. 检查文件中是否包含常见的代码标识符
            var codeIdentifiers = keywords.Where(k => k.Length >= 3 && 
                (char.IsUpper(k[0]) || // 可能是类名
                 k.Contains("_") || // 下划线命名
                 k.Any(c => char.IsUpper(c)))); // 驼峰命名

            foreach (var identifier in codeIdentifiers)
            {
                if (content.Contains(identifier))
                {
                    score += 2;
                }
            }

            return score;
        }
    }
}
