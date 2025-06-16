using System;
using System.IO;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System.Diagnostics;

namespace VSAIPluginNew.Services
{
    public class FileWriteService
    {
        private readonly DTE2 _dte;

        public FileWriteService(DTE2 dte)
        {
            _dte = dte;
        }

        // 更新当前打开的文档内容
        public void UpdateActiveDocumentContent(string newContent)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                if (_dte.ActiveDocument == null)
                {
                    Debug.WriteLine("没有活动文档");
                    return;
                }

                TextDocument textDoc = _dte.ActiveDocument.Object("TextDocument") as TextDocument;
                if (textDoc == null)
                {
                    Debug.WriteLine("活动文档不是文本文档");
                    return;
                }

                Debug.WriteLine($"更新活动文档: {_dte.ActiveDocument.FullName}");
                
                // 保存当前选择位置
                TextSelection selection = textDoc.Selection;
                int startLine = selection.TopPoint.Line;
                int startCol = selection.TopPoint.LineCharOffset;

                // 替换整个文档内容
                EditPoint startPoint = textDoc.StartPoint.CreateEditPoint();
                EditPoint endPoint = textDoc.EndPoint.CreateEditPoint();
                startPoint.Delete(endPoint);
                startPoint.Insert(newContent);

                // 尝试恢复选择位置
                try
                {
                    selection.MoveToLineAndOffset(startLine, startCol);
                }
                catch
                {
                    // 如果不能恢复选择位置，忽略错误
                    Debug.WriteLine("无法恢复选择位置");
                }
                
                Debug.WriteLine("文档更新成功");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"更新文档时出错: {ex.Message}");
                Debug.WriteLine($"错误详情: {ex}");
            }
        }

        // 更新指定文件的内容
        public async Task<bool> UpdateFileContentAsync(string filePath, string newContent)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(newContent))
                {
                    Debug.WriteLine("更新文件失败: 文件路径或内容为空");
                    return false;
                }
                
                Debug.WriteLine($"尝试更新文件: {filePath}");
                
                if (!File.Exists(filePath))
                {
                    // 如果文件路径不是绝对路径，尝试转换
                    if (!Path.IsPathRooted(filePath))
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        string solutionDir = Path.GetDirectoryName(_dte.Solution.FullName);
                        string absolutePath = Path.Combine(solutionDir, filePath);
                        
                        if (File.Exists(absolutePath))
                        {
                            filePath = absolutePath;
                            Debug.WriteLine($"找到解决方案中的文件: {filePath}");
                        }
                        else
                        {
                            Debug.WriteLine($"文件不存在: {filePath}");
                            
                            // 尝试创建文件
                            Debug.WriteLine($"尝试创建新文件: {filePath}");
                            return await CreateNewFileAsync(filePath, newContent);
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"文件不存在: {filePath}");
                        
                        // 尝试创建文件
                        Debug.WriteLine($"尝试创建新文件: {filePath}");
                        return await CreateNewFileAsync(filePath, newContent);
                    }
                }

                File.WriteAllText(filePath, newContent);
                Debug.WriteLine($"已更新文件: {filePath}");

                // 如果文件已经在VS中打开，刷新显示
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                
                foreach (Document doc in _dte.Documents)
                {
                    if (string.Equals(doc.FullName, filePath, StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.WriteLine($"在VS中重新加载文件: {filePath}");
                        try
                        {
                            doc.Close(vsSaveChanges.vsSaveChangesNo);
                            _dte.ItemOperations.OpenFile(filePath);
                        }
                        catch (Exception ex)
                        {
                            // 忽略关闭或重新打开错误
                            Debug.WriteLine($"重新加载文件时出错: {ex.Message}");
                        }
                        break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"更新文件 {filePath} 时出错: {ex.Message}");
                Debug.WriteLine($"错误详情: {ex}");
                return false;
            }
        }

        // 创建新文件并填入内容
        public async Task<bool> CreateNewFileAsync(string filePath, string content)
        {
            try
            {
                Debug.WriteLine($"正在创建新文件: {filePath}");
                
                // 确保目录存在
                string? directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Debug.WriteLine($"创建目录: {directory}");
                    Directory.CreateDirectory(directory);
                }

                // 写入文件内容
                File.WriteAllText(filePath, content);
                Debug.WriteLine($"已创建文件: {filePath}");
                
                // 尝试在Visual Studio中打开文件
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                try
                {
                    _dte.ItemOperations.OpenFile(filePath);
                    Debug.WriteLine($"已在VS中打开文件: {filePath}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"无法在VS中打开文件: {ex.Message}");
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"创建文件 {filePath} 时出错: {ex.Message}");
                Debug.WriteLine($"错误详情: {ex}");
                return false;
            }
        }
    }
} 