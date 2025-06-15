using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using VSAIPluginNew.Services;

namespace VSAIPluginNew.ViewModels
{
    public class AIAssistantViewModel : INotifyPropertyChanged
    {
        private readonly IAIService _aiService;
        private readonly IFileService _fileService;
        private CancellationTokenSource? _cancellationTokenSource;
        
        private string _userInput = string.Empty;
        private string _aiOutput = string.Empty;
        private string _selectedLanguage = "中文";
        private string _selectedTaskType = "综合任务";
        private bool _isProcessing;
        
        public event PropertyChangedEventHandler? PropertyChanged;

        public AIAssistantViewModel(IAIService aiService, IFileService fileService)
        {
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            
            InitializeAvailableOptions();
        }

        private void InitializeAvailableOptions()
        {
            // 初始化可用选项
            AvailableLanguages = new List<string> { "中文", "English" };
            AvailableTaskTypes = new List<string>
            {
                "综合任务",    // 使用大模型
                "代码分析",    // 使用小模型
                "代码生成",    // 使用小模型
                "文本总结",    // 使用小模型
                "复杂分析",    // 使用大模型
                "多文件处理",  // 使用大模型
                "架构设计"     // 使用大模型
            };
        }

        public List<string> AvailableLanguages { get; private set; } = new();
        public List<string> AvailableTaskTypes { get; private set; } = new();

        public string UserInput
        {
            get => _userInput;
            set => SetProperty(ref _userInput, value);
        }

        public string AIOutput
        {
            get => _aiOutput;
            set => SetProperty(ref _aiOutput, value);
        }

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
        }

        public string SelectedTaskType
        {
            get => _selectedTaskType;
            set => SetProperty(ref _selectedTaskType, value);
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }

        /// <summary>
        /// Initializes the view model asynchronously.
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                IsProcessing = true;
                await _fileService.LoadSolutionFilesAsync();
            }
            finally
            {
                IsProcessing = false;
            }
        }

        public async Task SubmitQueryAsync()
        {
            if (string.IsNullOrWhiteSpace(UserInput))
                return;

            try
            {
                // 创建新的取消令牌源
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();
                
                IsProcessing = true;
                AIOutput = "正在处理查询...";

                var contextFiles = await _fileService.GetRelevantFilesAsync(UserInput);
                AIOutput = await _aiService.ProcessQueryAsync(
                    UserInput,
                    SelectedTaskType,
                    contextFiles,
                    _cancellationTokenSource.Token
                );
            }
            catch (OperationCanceledException)
            {
                AIOutput += "\n\n操作已取消。";
            }
            catch (Exception ex)
            {
                AIOutput = $"处理查询时出错: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        public void CancelCurrentOperation()
        {
            _cancellationTokenSource?.Cancel();
            IsProcessing = false;
            AIOutput += "\n\n操作已取消。";
        }

        public void CopyOutput()
        {
            if (!string.IsNullOrEmpty(AIOutput))
            {
                System.Windows.Forms.Clipboard.SetText(AIOutput);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName ?? string.Empty));
        }

        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
    }
}
