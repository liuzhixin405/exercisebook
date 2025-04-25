using System;
using System.IO;
using System.Threading.Tasks;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

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
                    return;

                TextDocument textDoc = _dte.ActiveDocument.Object("TextDocument") as TextDocument;
                if (textDoc == null)
                    return;

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
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating document: {ex.Message}");
            }
        }

        // 更新指定文件的内容
        public async Task<bool> UpdateFileContentAsync(string filePath, string newContent)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                 File.WriteAllText(filePath, newContent);

                // 如果文件已经在VS中打开，刷新显示
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                
                foreach (Document doc in _dte.Documents)
                {
                    if (string.Equals(doc.FullName, filePath, StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            doc.Close(vsSaveChanges.vsSaveChangesNo);
                            _dte.ItemOperations.OpenFile(filePath);
                        }
                        catch
                        {
                            // 忽略关闭或重新打开错误
                        }
                        break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error writing to file {filePath}: {ex.Message}");
                return false;
            }
        }

        // 创建新文件并填入内容
        public async Task<bool> CreateNewFileAsync(string filePath, string content)
        {
            try
            {
                // 确保目录存在
                string? directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 写入文件内容
                 File.WriteAllText(filePath, content);

                // 尝试在VS中打开文件
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                _dte.ItemOperations.OpenFile(filePath);

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating file {filePath}: {ex.Message}");
                return false;
            }
        }
    }
} 