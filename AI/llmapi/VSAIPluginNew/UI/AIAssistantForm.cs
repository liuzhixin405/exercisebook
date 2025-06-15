using System;
using System.ComponentModel;
using System.Windows.Forms;
using VSAIPluginNew.ViewModels;

namespace VSAIPluginNew.UI
{
    public partial class AIAssistantForm : Form
    {
        private readonly AIAssistantViewModel _viewModel;
        private Button? cancelButton;

        public AIAssistantForm(AIAssistantViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            InitializeComponent();
            AddCancelButton();
            InitializeBindings();
            
            // 确保输出文本框正确初始化和显示
            aiOutputTextBox.Visible = true;
            aiOutputTextBox.Enabled = true;
            aiOutputTextBox.Text = string.Empty;
            
            this.Load += (s, e) => {
                aiOutputTextBox.BringToFront();
                aiOutputTextBox.Refresh();
            };
        }

        private void AddCancelButton()
        {
            // 添加取消按钮
            cancelButton = new Button
            {
                Text = "取消",
                Width = 75,
                Height = 23,
                Visible = false,
                Enabled = false
            };
            
            // 将取消按钮和其他按钮添加到按钮面板
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Height = 35,
                Padding = new Padding(0, 5, 5, 5)
            };
            
            // 将现有按钮添加到面板
            buttonPanel.Controls.Add(cancelButton);
            buttonPanel.Controls.Add(submitButton);
            buttonPanel.Controls.Add(copyButton);

            // 从原位置移除按钮
            this.Controls.Remove(submitButton);
            this.Controls.Remove(copyButton);

            // 添加面板到窗体
            this.Controls.Add(buttonPanel);
        }

        private void InitializeBindings()
        {
            if (cancelButton == null || submitButton == null || copyButton == null)
            {
                throw new InvalidOperationException("按钮未正确初始化");
            }

            // 绑定ComboBox数据源
            languageComboBox.DataSource = _viewModel.AvailableLanguages;
            taskTypeComboBox.DataSource = _viewModel.AvailableTaskTypes;

            // 绑定选择值
            languageComboBox.SelectedIndexChanged += (s, e) => {
                if (languageComboBox.SelectedItem != null)
                    _viewModel.SelectedLanguage = languageComboBox.SelectedItem.ToString() ?? string.Empty;
            };
            
            taskTypeComboBox.SelectedIndexChanged += (s, e) => {
                if (taskTypeComboBox.SelectedItem != null)
                    _viewModel.SelectedTaskType = taskTypeComboBox.SelectedItem.ToString() ?? string.Empty;
            };

            // 双向绑定用户输入
            userInputTextBox.TextChanged += (s, e) => _viewModel.UserInput = userInputTextBox.Text;
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;

            // 绑定按钮事件
            submitButton.Click += async (s, e) => {
                submitButton.Enabled = false;
                cancelButton.Visible = true;
                cancelButton.Enabled = true;
                try {
                    await _viewModel.SubmitQueryAsync();
                }
                finally {
                    submitButton.Enabled = true;
                    cancelButton.Visible = false;
                    cancelButton.Enabled = false;
                }
            };
            
            copyButton.Click += (s, e) => _viewModel.CopyOutput();
            
            cancelButton.Click += (s, e) => {
                _viewModel.CancelCurrentOperation();
                cancelButton.Enabled = false;
            };
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => ViewModel_PropertyChanged(sender, e)));
                return;
            }

            if (cancelButton == null) return;

            switch (e.PropertyName)
            {
                case nameof(AIAssistantViewModel.AIOutput):
                    aiOutputTextBox.Text = _viewModel.AIOutput;
                    aiOutputTextBox.Refresh();
                    break;
                case nameof(AIAssistantViewModel.IsProcessing):
                    submitButton.Enabled = !_viewModel.IsProcessing;
                    cancelButton.Visible = _viewModel.IsProcessing;
                    cancelButton.Enabled = _viewModel.IsProcessing;
                    break;
            }
        }
    }
}
