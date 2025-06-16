using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSAIPluginNew.Commands;
using Task = System.Threading.Tasks.Task;

namespace VSAIPluginNew
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(VSAIPluginNewPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class VSAIPluginNewPackage : AsyncPackage
    {
        /// <summary>
        /// VSAIPluginNewPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "30f3f997-7016-4a07-bae8-57930b7def3c";

        /// <summary>
        /// 命令集GUID字符串
        /// </summary>
        public const string CommandSetGuidString = "6ff9e33a-5ec4-4e32-a5a7-f5e3c78c9d1b";

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // 当我们在后台初始化异步包（Background Load)时，需要等待UI线程来执行这些UI相关的操作。
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            
            // 获取DTE服务
            DTE2 dte = await GetServiceAsync(typeof(DTE)) as DTE2;
            
            // 初始化命令
            await AIAssistantCommand.InitializeAsync(this, dte);

            // 向状态栏添加欢迎消息
            var statusBar = await GetServiceAsync(typeof(SVsStatusbar)) as IVsStatusbar;
            if (statusBar != null)
            {
                object icon = (short)Microsoft.VisualStudio.Shell.Interop.Constants.SBAI_Build;
                statusBar.Animation(1, ref icon); // 开始动画
                statusBar.SetText("AI助手已加载");
                await Task.Delay(3000);
                statusBar.Animation(0, ref icon); // 停止动画
                statusBar.Clear();
            }
        }

        #endregion
    }
} 