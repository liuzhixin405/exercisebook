using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using VSAIPluginNew.Services;
using VSAIPluginNew.ViewModels;
using VSAIPluginNew.UI;
using Task = System.Threading.Tasks.Task;

namespace VSAIPluginNew.Commands
{
    /// <summary>
    /// AI助手命令处理类
    /// </summary>
    internal sealed class AIAssistantCommand
    {
        /// <summary>
        /// 命令ID
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// 命令菜单组GUID
        /// </summary>
        public static readonly Guid CommandSet = new Guid("6ff9e33a-5ec4-4e32-a5a7-f5e3c78c9d1b");

        private readonly AsyncPackage package;
        private readonly DTE2 _dte;
        private readonly IAIService _aiService;
        private readonly IFileService _fileService;

        /// <summary>
        /// 命令实例
        /// </summary>
        public static AIAssistantCommand Instance { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        private AIAssistantCommand(AsyncPackage package, OleMenuCommandService commandService, DTE2 dte)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            _dte = dte ?? throw new ArgumentNullException(nameof(dte));

            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(HandleCommand, menuCommandID);
                commandService.AddCommand(menuItem);
            }

            // 初始化服务
            _aiService = new AIService();
            _fileService = new FileService(_dte);
        }

        /// <summary>
        /// 初始化命令单例
        /// </summary>
        public static async Task InitializeAsync(AsyncPackage package, DTE2 dte)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new AIAssistantCommand(package, commandService, dte);
        }

        /// <summary>
        /// 处理命令执行
        /// </summary>
        private void HandleCommand(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            _ = HandleCommandAsync(sender, e);
        }

        /// <summary>
        /// 异步执行命令
        /// </summary>
        private async Task HandleCommandAsync(object sender, EventArgs e)
        {
            try
            {
                // 切换到UI线程
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                // 验证环境
                if (_dte == null)
                {
                    MessageBox.Show(
                        "无法获取DTE服务",
                        "错误",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // 初始化视图模型
                var viewModel = new AIAssistantViewModel(_aiService, _fileService);
                
                try
                {
                    // 预加载解决方案文件信息
                    await viewModel.InitializeAsync();
                }
                catch (Exception initEx)
                {
                    System.Diagnostics.Debug.WriteLine($"初始化视图模型时出错: {initEx.Message}");
                    // 继续执行，因为这不是致命错误
                }

                // 显示UI
                var form = new AIAssistantForm(viewModel);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"执行AI助手命令时出错: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                MessageBox.Show(
                    $"执行命令时出错:\n{ex.Message}",
                    "错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
