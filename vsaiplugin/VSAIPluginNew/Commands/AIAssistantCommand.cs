using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSAIPluginNew.AI;
using VSAIPluginNew.Services;
using Task = System.Threading.Tasks.Task;

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

                    // 调用AI生成回复
                    var agent = AgentFactory.GetOllamaAgent();
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
                    form.Width = 600;
                    form.Height = 300;
                    form.StartPosition = FormStartPosition.CenterScreen;

                    var promptLabel = new Label
                    {
                        Text = "在下方输入你的问题或需求:",
                        Left = 10,
                        Top = 10,
                        Width = 580
                    };
                    form.Controls.Add(promptLabel);

                    var promptTextBox = new TextBox
                    {
                        Left = 10,
                        Top = 30,
                        Width = 560,
                        Height = 180,
                        Multiline = true,
                        ScrollBars = ScrollBars.Vertical
                    };
                    form.Controls.Add(promptTextBox);

                    var submitButton = new Button
                    {
                        Text = "提交",
                        Left = 400,
                        Top = 220,
                        Width = 80
                    };
                    submitButton.Click += (s, e) =>
                    {
                        form.DialogResult = DialogResult.OK;
                    };
                    form.Controls.Add(submitButton);

                    var cancelButton = new Button
                    {
                        Text = "取消",
                        Left = 490,
                        Top = 220,
                        Width = 80
                    };
                    cancelButton.Click += (s, e) =>
                    {
                        form.DialogResult = DialogResult.Cancel;
                    };
                    form.Controls.Add(cancelButton);

                    if (form.ShowDialog() == DialogResult.OK)
                    {
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

            using (var form = new Form())
            {
                form.Text = "AI助手回复";
                form.Width = 800;
                form.Height = 600;
                form.StartPosition = FormStartPosition.CenterScreen;

                var responseTextBox = new TextBox
                {
                    Left = 10,
                    Top = 10,
                    Width = 760,
                    Height = 500,
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Both,
                    Text = response,
                    Font = new System.Drawing.Font("Consolas", 10)
                };
                form.Controls.Add(responseTextBox);

                var applyChangesButton = new Button
                {
                    Text = "应用建议的更改",
                    Left = 500,
                    Top = 520,
                    Width = 130
                };
                applyChangesButton.Click += async (s, e) =>
                {
                    try
                    {
                        // 实现代码更改逻辑
                        // 这里需要解析AI回复，找出特定的代码更改建议并应用
                        await ApplyCodeChangesAsync(response);
                        form.Close();
                    }
                    catch (Exception ex)
                    {
                        await ShowMessageAsync($"应用更改时出错: {ex.Message}");
                    }
                };
                form.Controls.Add(applyChangesButton);

                var closeButton = new Button
                {
                    Text = "关闭",
                    Left = 640,
                    Top = 520,
                    Width = 130
                };
                closeButton.Click += (s, e) => form.Close();
                form.Controls.Add(closeButton);

                form.ShowDialog();
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