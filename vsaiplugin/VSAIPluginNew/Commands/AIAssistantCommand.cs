using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSAIPluginNew.AI;
using VSAIPluginNew.Services;
using Task = System.Threading.Tasks.Task;
using System.Collections.Generic;
using System.Linq;

namespace VSAIPluginNew.Commands
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class AIAssistantCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("6ff9e33a-5ec4-4e32-a5a7-f5e3c78c9d1b");

        /// <summary>
        /// VS Package that provides this command
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// DTE2 实例
        /// </summary>
        private DTE2 _dte;

        /// <summary>
        /// 文件访问服务
        /// </summary>
        private FileAccessService _fileAccessService;

        /// <summary>
        /// 文件写入服务
        /// </summary>
        private FileWriteService _fileWriteService;

        /// <summary>
        /// 存储用户最后一次提问
        /// </summary>
        private string _lastUserPrompt;

        /// <summary>
        /// 存储用户选择的语言（中文或英文）
        /// </summary>
        private string _selectedLanguage = "中文";

        /// <summary>
        /// 存储用户选择的模型
        /// </summary>
        private string _selectedModel = "phi4-mini:latest";

        /// <summary>
        /// Initializes a new instance of the <see cref="AIAssistantCommand"/> class.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        /// <param name="dte">DTE2 service</param>
        private AIAssistantCommand(AsyncPackage package, OleMenuCommandService commandService, DTE2 dte)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            _dte = dte ?? throw new ArgumentNullException(nameof(dte));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);

            // 初始化服务
            _fileAccessService = new FileAccessService(_dte);
            _fileWriteService = new FileWriteService(_dte);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static AIAssistantCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="dte">DTE2 service</param>
        public static async Task InitializeAsync(AsyncPackage package, DTE2 dte)
        {
            // 切换到UI线程
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService == null || dte == null)
                return;

            Instance = new AIAssistantCommand(package, commandService, dte);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Task.Run(async () =>
            {
                try
                {
                    // 检查Ollama是否运行
                    if (!await AgentFactory.IsOllamaRunningAsync())
                    {
                        await ShowMessageAsync("Ollama服务未运行。尝试启动中...");
                        AgentFactory.TryStartOllama();
                        
                        // 等待几秒钟让Ollama启动
                        await Task.Delay(5000);
                        
                        if (!await AgentFactory.IsOllamaRunningAsync())
                        {
                            await ShowMessageAsync("无法启动Ollama服务。请确保已安装Ollama并尝试手动启动。");
                            return;
                        }
                    }

                    // 获取用户输入
                    string prompt = await RequestUserPromptAsync();
                    if (string.IsNullOrEmpty(prompt))
                        return;

                    // 显示状态信息
                    await ShowMessageAsync("正在处理请求...");

                    // 获取所有解决方案文件内容
                    var filesContent = await _fileAccessService.GetAllSolutionFilesContentAsync();

                    // 根据用户选择的语言修改提示
                    string languageInstruction = _selectedLanguage == "中文" 
                        ? "请用中文回答以下问题:" 
                        : "Please answer the following question in English:";
                    
                    prompt = $"{languageInstruction}\n{prompt}";

                    // 调用AI生成回复，使用用户选择的模型
                    var agent = AgentFactory.GetOllamaAgent();
                    // 更新模型名称
                    agent.UpdateModel(_selectedModel);
                    string response = await agent.ProcessFilesAndGenerateReplyAsync(prompt, filesContent);

                    // 显示结果
                    await ShowResultAsync(response);
                }
                catch (Exception ex)
                {
                    await ShowMessageAsync($"发生错误: {ex.Message}");
                }
            });
        }

        // 请求用户输入提示
        private Task<string> RequestUserPromptAsync()
        {
            var tcs = new TaskCompletionSource<string>();

            ThreadHelper.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                using (var form = new Form())
                {
                    form.Text = "AI助手";
                    form.Width = 700;
                    form.Height = 450; // 增加高度以容纳新控件
                    form.StartPosition = FormStartPosition.CenterScreen;
                    form.MinimizeBox = true;
                    form.MaximizeBox = true;
                    form.FormBorderStyle = FormBorderStyle.Sizable;

                    var promptLabel = new Label
                    {
                        Text = "在下方输入你的问题或需求:",
                        Left = 10,
                        Top = 10,
                        Width = 680,
                        Font = new System.Drawing.Font("微软雅黑", 10, System.Drawing.FontStyle.Bold)
                    };
                    form.Controls.Add(promptLabel);

                    var promptTextBox = new RichTextBox
                    {
                        Left = 10,
                        Top = 40,
                        Width = 670,
                        Height = 250, // 减小高度以容纳新控件
                        Font = new System.Drawing.Font("微软雅黑", 10),
                        BorderStyle = BorderStyle.FixedSingle,
                        AcceptsTab = true,
                        AutoSize = false,
                        Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
                    };
                    promptTextBox.ScrollBars = RichTextBoxScrollBars.Vertical;
                    promptTextBox.DetectUrls = true;
                    form.Controls.Add(promptTextBox);

                    // 添加语言选择
                    var languageLabel = new Label
                    {
                        Text = "选择回复语言:",
                        Left = 10,
                        Top = 300,
                        Width = 120,
                        Font = new System.Drawing.Font("微软雅黑", 9)
                    };
                    form.Controls.Add(languageLabel);

                    var languageComboBox = new ComboBox
                    {
                        Left = 130,
                        Top = 298,
                        Width = 120,
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Font = new System.Drawing.Font("微软雅黑", 9)
                    };
                    languageComboBox.Items.AddRange(new object[] { "中文", "English" });
                    languageComboBox.SelectedItem = _selectedLanguage;
                    form.Controls.Add(languageComboBox);

                    // 添加模型选择
                    var modelLabel = new Label
                    {
                        Text = "选择AI模型:",
                        Left = 270,
                        Top = 300,
                        Width = 100,
                        Font = new System.Drawing.Font("微软雅黑", 9)
                    };
                    form.Controls.Add(modelLabel);

                    var modelComboBox = new ComboBox
                    {
                        Left = 370,
                        Top = 298,
                        Width = 200,
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Font = new System.Drawing.Font("微软雅黑", 9)
                    };
                    form.Controls.Add(modelComboBox);

                    // 加载模型列表按钮
                    var refreshModelButton = new Button
                    {
                        Text = "刷新",
                        Left = 575,
                        Top = 297,
                        Width = 60,
                        Height = 25,
                        Font = new System.Drawing.Font("微软雅黑", 8)
                    };
                    refreshModelButton.Click += async (s, e) =>
                    {
                        try
                        {
                            refreshModelButton.Enabled = false;
                            refreshModelButton.Text = "加载中...";
                            modelComboBox.Items.Clear();
                            
                            // 获取本地模型列表
                            var models = await GetLocalModelsAsync();
                            
                            if (models.Count > 0)
                            {
                                foreach (var model in models)
                                {
                                    modelComboBox.Items.Add(model);
                                }
                                // 如果之前选择的模型在列表中，则保持选中，否则选择第一个
                                if (models.Contains(_selectedModel))
                                {
                                    modelComboBox.SelectedItem = _selectedModel;
                                }
                                else
                                {
                                    modelComboBox.SelectedIndex = 0;
                                }
                            }
                            else
                            {
                                modelComboBox.Items.Add("phi4-mini:latest");
                                modelComboBox.SelectedIndex = 0;
                                MessageBox.Show("未找到本地模型或无法获取模型列表。\n请确保Ollama已安装并有可用模型。",
                                    "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"获取模型列表失败: {ex.Message}",
                                "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            modelComboBox.Items.Add("phi4-mini:latest");
                            modelComboBox.SelectedIndex = 0;
                        }
                        finally
                        {
                            refreshModelButton.Enabled = true;
                            refreshModelButton.Text = "刷新";
                        }
                    };
                    form.Controls.Add(refreshModelButton);

                    // 加载默认模型
                    modelComboBox.Items.Add(_selectedModel);
                    modelComboBox.SelectedIndex = 0;
                    
                    // 尝试自动加载模型列表
                    refreshModelButton.PerformClick();

                    var buttonPanel = new Panel
                    {
                        Left = 0,
                        Top = 330,
                        Width = 700,
                        Height = 70,
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
                    };
                    form.Controls.Add(buttonPanel);

                    var submitButton = new Button
                    {
                        Text = "提交",
                        Width = 100,
                        Height = 30,
                        Left = 460,
                        Top = 20,
                        Font = new System.Drawing.Font("微软雅黑", 9),
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                    };
                    submitButton.Click += (s, e) =>
                    {
                        // 保存选择的语言和模型
                        _selectedLanguage = languageComboBox.SelectedItem?.ToString() ?? "中文";
                        _selectedModel = modelComboBox.SelectedItem?.ToString() ?? "phi4-mini:latest";
                        form.DialogResult = DialogResult.OK;
                    };
                    buttonPanel.Controls.Add(submitButton);

                    var cancelButton = new Button
                    {
                        Text = "取消",
                        Width = 100,
                        Height = 30,
                        Left = 570,
                        Top = 20,
                        Font = new System.Drawing.Font("微软雅黑", 9),
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                    };
                    cancelButton.Click += (s, e) =>
                    {
                        form.DialogResult = DialogResult.Cancel;
                    };
                    buttonPanel.Controls.Add(cancelButton);

                    // 按下Enter+Ctrl键提交
                    promptTextBox.KeyDown += (s, e) =>
                    {
                        if (e.Control && e.KeyCode == Keys.Enter)
                        {
                            // 保存选择的语言和模型
                            _selectedLanguage = languageComboBox.SelectedItem?.ToString() ?? "中文";
                            _selectedModel = modelComboBox.SelectedItem?.ToString() ?? "phi4-mini:latest";
                            form.DialogResult = DialogResult.OK;
                        }
                    };

                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        _lastUserPrompt = promptTextBox.Text; // 保存用户提问
                        tcs.SetResult(promptTextBox.Text);
                    }
                    else
                    {
                        tcs.SetResult(string.Empty);
                    }
                }
            });

            return tcs.Task;
        }

        // 获取本地模型列表
        private async Task<List<string>> GetLocalModelsAsync()
        {
            var models = new List<string>();
            
            try
            {
                // 创建进程调用ollama list命令
                using (var process = new System.Diagnostics.Process())
                {
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.Arguments = "/c ollama list";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();
                    
                    // 读取输出
                    string output = await process.StandardOutput.ReadToEndAsync();
                    // 等待进程结束
                    process.WaitForExit();
                    
                    if (process.ExitCode == 0)
                    {
                        // 解析模型列表
                        string[] lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        
                        // 跳过第一行（通常是表头）
                        for (int i = 1; i < lines.Length; i++)
                        {
                            string line = lines[i].Trim();
                            if (!string.IsNullOrEmpty(line))
                            {
                                // ollama list输出格式: NAME TAG SIZE ...
                                // 我们需要NAME:TAG的格式
                                string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length >= 2)
                                {
                                    string modelName = $"{parts[0]}:{parts[1]}";
                                    models.Add(modelName);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"获取模型列表出错: {ex.Message}");
            }
            
            return models;
        }

        // 显示消息
        private async Task ShowMessageAsync(string message)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            VsShellUtilities.ShowMessageBox(
                this.package,
                message,
                "AI助手",
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        // 显示AI结果
        private async Task ShowResultAsync(string response)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            // 保存最后一次用户提问内容
            string userPrompt = _lastUserPrompt ?? "无法显示问题内容";

            // 检查响应是否为空
            if (string.IsNullOrWhiteSpace(response))
            {
                response = "AI助手未返回任何内容，请检查网络连接或重试。";
            }

            using (var form = new Form())
            {
                form.Text = "AI助手对话";
                form.Width = 900;
                form.Height = 700;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.MinimizeBox = true;
                form.MaximizeBox = true;
                form.FormBorderStyle = FormBorderStyle.Sizable;
                form.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);

                // 使用TableLayoutPanel布局
                var mainLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 3,
                    Padding = new Padding(10),
                    CellBorderStyle = TableLayoutPanelCellBorderStyle.None
                };
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F)); // 问题区域
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 70F)); // 回复区域
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F)); // 按钮区域
                form.Controls.Add(mainLayout);

                // 用户问题区域 
                var userPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(10)
                };
                mainLayout.Controls.Add(userPanel, 0, 0);

                // 问题标题
                var userLabel = new Label
                {
                    Text = "你的问题:",
                    Dock = DockStyle.Top,
                    Font = new System.Drawing.Font("微软雅黑", 10, System.Drawing.FontStyle.Bold),
                    Height = 30,
                    ForeColor = System.Drawing.Color.FromArgb(0, 80, 0)
                };
                userPanel.Controls.Add(userLabel);

                // 问题文本框
                var userTextBox = new RichTextBox
                {
                    Text = userPrompt,
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = System.Drawing.Color.FromArgb(240, 255, 240),
                    Font = new System.Drawing.Font("微软雅黑", 10),
                    ScrollBars = RichTextBoxScrollBars.Vertical
                };
                userPanel.Controls.Add(userTextBox);

                // AI回复区域
                var aiPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(10)
                };
                mainLayout.Controls.Add(aiPanel, 0, 1);

                // 回复标题
                var aiLabel = new Label
                {
                    Text = "AI助手回复:",
                    Dock = DockStyle.Top,
                    Font = new System.Drawing.Font("微软雅黑", 10, System.Drawing.FontStyle.Bold),
                    Height = 30,
                    ForeColor = System.Drawing.Color.FromArgb(0, 0, 120)
                };
                aiPanel.Controls.Add(aiLabel);

                // 回复文本框
                var aiTextBox = new RichTextBox
                {
                    Text = response,
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = System.Drawing.Color.White,
                    Font = new System.Drawing.Font("Consolas", 10),
                    ScrollBars = RichTextBoxScrollBars.Both,
                    WordWrap = true,
                    DetectUrls = true
                };
                HighlightCodeBlocks(aiTextBox); // 高亮代码区域
                aiPanel.Controls.Add(aiTextBox);

                // 按钮区域
                var buttonPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.RightToLeft,
                    WrapContents = false,
                    Padding = new Padding(10, 10, 10, 10)
                };
                mainLayout.Controls.Add(buttonPanel, 0, 2);

                // 关闭按钮
                var closeButton = new Button
                {
                    Text = "关闭",
                    Width = 100,
                    Height = 35,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = System.Drawing.Color.FromArgb(230, 230, 230),
                    Font = new System.Drawing.Font("微软雅黑", 9),
                    Cursor = Cursors.Hand,
                    Margin = new Padding(5)
                };
                closeButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(200, 200, 200);
                closeButton.Click += (s, e) => form.Close();
                buttonPanel.Controls.Add(closeButton);

                // 应用更改按钮
                var applyChangesButton = new Button
                {
                    Text = "应用建议的更改",
                    Width = 150,
                    Height = 35,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = System.Drawing.Color.FromArgb(0, 120, 215),
                    ForeColor = System.Drawing.Color.White,
                    Font = new System.Drawing.Font("微软雅黑", 9),
                    Cursor = Cursors.Hand,
                    Margin = new Padding(5)
                };
                applyChangesButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(0, 100, 200);
                applyChangesButton.Click += async (s, e) =>
                {
                    try
                    {
                        await ApplyCodeChangesAsync(response);
                        form.Close();
                    }
                    catch (Exception ex)
                    {
                        await ShowMessageAsync($"应用更改时出错: {ex.Message}");
                    }
                };
                buttonPanel.Controls.Add(applyChangesButton);

                // 复制按钮
                var copyButton = new Button
                {
                    Text = "复制回复",
                    Width = 120,
                    Height = 35,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = System.Drawing.Color.FromArgb(230, 230, 230),
                    Font = new System.Drawing.Font("微软雅黑", 9),
                    Cursor = Cursors.Hand,
                    Margin = new Padding(5)
                };
                copyButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(200, 200, 200);
                copyButton.Click += (s, e) =>
                {
                    try
                    {
                        Clipboard.SetText(response);
                        MessageBox.Show("已复制到剪贴板", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"复制失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };
                buttonPanel.Controls.Add(copyButton);

                // 显示对话窗口
                form.ShowDialog();
            }
        }

        // 为代码块添加简单的语法高亮
        private void HighlightCodeBlocks(RichTextBox rtb)
        {
            string text = rtb.Text;
            int startIndex = 0;
            bool inCodeBlock = false;
            
            for (int i = 0; i < text.Length - 2; i++)
            {
                if (i + 3 <= text.Length && text.Substring(i, 3) == "```")
                {
                    if (!inCodeBlock)
                    {
                        inCodeBlock = true;
                        startIndex = i;
                    }
                    else
                    {
                        inCodeBlock = false;
                        
                        try
                        {
                            // 高亮代码块
                            rtb.SelectionStart = startIndex;
                            rtb.SelectionLength = i + 3 - startIndex;
                            rtb.SelectionBackColor = System.Drawing.Color.FromArgb(240, 240, 240);
                            rtb.SelectionFont = new System.Drawing.Font("Consolas", 10);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"高亮代码块出错: {ex.Message}");
                        }
                    }
                }
            }
        }

        // 应用代码更改
        private async Task ApplyCodeChangesAsync(string aiResponse)
        {
            // 这里需要一个基本的解析器来从AI回复中提取代码更改建议
            // 这是一个简单的实现示例，实际应用中可能需要更复杂的解析

            // 检查是否包含代码块
            if (!aiResponse.Contains("```"))
            {
                await ShowMessageAsync("未找到可应用的代码更改。");
                return;
            }

            // 寻找代码块和文件名
            var lines = aiResponse.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            string currentFile = null;
            bool inCodeBlock = false;
            var codeBuilder = new System.Text.StringBuilder();

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                // 检查是否是文件路径指示
                if (line.Contains("文件:") || line.Contains("File:"))
                {
                    var fileParts = line.Split(new[] { ':' }, 2);
                    if (fileParts.Length == 2)
                    {
                        currentFile = fileParts[1].Trim();
                    }
                }

                // 代码块开始
                if (line.StartsWith("```"))
                {
                    if (!inCodeBlock)
                    {
                        inCodeBlock = true;
                        codeBuilder.Clear();
                        continue;
                    }
                    else
                    {
                        inCodeBlock = false;
                        
                        // 如果有文件路径和代码内容，尝试更新文件
                        if (!string.IsNullOrEmpty(currentFile) && codeBuilder.Length > 0)
                        {
                            await _fileWriteService.UpdateFileContentAsync(currentFile, codeBuilder.ToString());
                            await ShowMessageAsync($"已更新文件: {currentFile}");
                        }
                        
                        currentFile = null;
                        continue;
                    }
                }

                // 在代码块内，累积代码内容
                if (inCodeBlock)
                {
                    codeBuilder.AppendLine(line);
                }
            }
        }
    }
} 