using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace VSAIPluginNew.Services
{
    public class FileAccessService
    {
        private readonly DTE2 _dte;

        public FileAccessService(DTE2 dte)
        {
            _dte = dte;
        }

        // 获取当前打开的文档内容
        public string GetActiveDocumentContent()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_dte.ActiveDocument == null)
                return string.Empty;

            TextDocument textDoc = _dte.ActiveDocument.Object("TextDocument") as TextDocument;
            if (textDoc == null)
                return string.Empty;

            EditPoint startPoint = textDoc.StartPoint.CreateEditPoint();
            return startPoint.GetText(textDoc.EndPoint);
        }

        // 获取当前解决方案中的所有文件
        public async Task<Dictionary<string, string>> GetAllSolutionFilesContentAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var result = new Dictionary<string, string>();
            if (_dte.Solution == null)
                return result;

            var fileList = GetAllProjectItems(_dte.Solution.Projects.Cast<Project>().ToArray());
            
            foreach (var file in fileList)
            {
                try
                {
                    if (File.Exists(file))
                    {
                        result[file] =  File.ReadAllText(file);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error reading file {file}: {ex.Message}");
                }
            }

            return result;
        }

        // 递归获取项目中的所有文件
        private List<string> GetAllProjectItems(Project[] projects)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var fileList = new List<string>();

            foreach (Project project in projects)
            {
                try
                {
                    if (project == null || string.IsNullOrEmpty(project.FullName))
                        continue;

                    // 处理文件夹项目
                    if (project.ProjectItems != null)
                    {
                        foreach (ProjectItem item in project.ProjectItems)
                        {
                            GetProjectItemFiles(item, fileList);
                        }
                    }

                    // 处理项目引用
                    if (project.ProjectItems != null)
                    {
                        foreach (ProjectItem item in project.ProjectItems)
                        {
                            if (item.SubProject != null)
                            {
                                GetAllProjectItems(new Project[] { item.SubProject });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error processing project {project.Name}: {ex.Message}");
                }
            }

            return fileList;
        }

        // 递归获取项目项中的所有文件
        private void GetProjectItemFiles(ProjectItem item, List<string> fileList)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                if (item == null)
                    return;

                // 获取文件路径
                if (item.FileCount > 0)
                {
                    try
                    {
                        // 只处理代码文件
                        string fileName = item.FileNames[0];
                        if (IsSupportedCodeFile(fileName) && !fileList.Contains(fileName))
                        {
                            fileList.Add(fileName);
                        }
                    }
                    catch { /* 忽略访问错误 */ }
                }

                // 递归处理子项
                if (item.ProjectItems != null)
                {
                    foreach (ProjectItem subItem in item.ProjectItems)
                    {
                        GetProjectItemFiles(subItem, fileList);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error processing project item: {ex.Message}");
            }
        }

        // 判断是否是支持的代码文件
        private bool IsSupportedCodeFile(string filePath)
        {
            // 支持常见的代码文件类型
            string[] supportedExtensions = {
                ".cs", ".vb", ".cpp", ".h", ".hpp", ".c", ".xaml", ".xml", ".json", ".csproj", ".vbproj", ".sln",
                ".js", ".ts", ".html", ".css", ".cshtml", ".razor", ".config", ".settings", ".resx"
            };

            return supportedExtensions.Any(ext => filePath.EndsWith(ext, StringComparison.OrdinalIgnoreCase));
        }
    }
} 