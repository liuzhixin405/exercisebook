namespace VSAIPluginNew.UI
{
    partial class AIAssistantForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.ComboBox languageComboBox;
        private System.Windows.Forms.ComboBox taskTypeComboBox;
        private System.Windows.Forms.Button submitButton;
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.TextBox userInputTextBox;
        private System.Windows.Forms.TextBox aiOutputTextBox;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.languageComboBox = new System.Windows.Forms.ComboBox();
            this.taskTypeComboBox = new System.Windows.Forms.ComboBox();
            this.submitButton = new System.Windows.Forms.Button();
            this.copyButton = new System.Windows.Forms.Button();
            this.userInputTextBox = new System.Windows.Forms.TextBox();
            this.aiOutputTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            
            // languageComboBox
            this.languageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.languageComboBox.Location = new System.Drawing.Point(12, 12);
            this.languageComboBox.Name = "languageComboBox";
            this.languageComboBox.Size = new System.Drawing.Size(100, 21);
            
            // taskTypeComboBox
            this.taskTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.taskTypeComboBox.Location = new System.Drawing.Point(118, 12);
            this.taskTypeComboBox.Name = "taskTypeComboBox";
            this.taskTypeComboBox.Size = new System.Drawing.Size(200, 21);
            
            // userInputTextBox
            this.userInputTextBox.Location = new System.Drawing.Point(12, 39);
            this.userInputTextBox.Multiline = true;
            this.userInputTextBox.Name = "userInputTextBox";
            this.userInputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.userInputTextBox.Size = new System.Drawing.Size(612, 150);
            
            // aiOutputTextBox
            this.aiOutputTextBox.Location = new System.Drawing.Point(12, 224);
            this.aiOutputTextBox.Multiline = true;
            this.aiOutputTextBox.Name = "aiOutputTextBox";
            this.aiOutputTextBox.ReadOnly = true;
            this.aiOutputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.aiOutputTextBox.Size = new System.Drawing.Size(612, 150);
            this.aiOutputTextBox.TabIndex = 5;
            this.aiOutputTextBox.Visible = true;
            this.aiOutputTextBox.Enabled = true;
            
            // submitButton
            this.submitButton.Location = new System.Drawing.Point(12, 195);
            this.submitButton.Name = "submitButton";
            this.submitButton.Size = new System.Drawing.Size(75, 23);
            this.submitButton.Text = "提交";
            this.submitButton.UseVisualStyleBackColor = true;
            
            // copyButton
            this.copyButton.Location = new System.Drawing.Point(93, 195);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(75, 23);
            this.copyButton.Text = "复制";
            this.copyButton.UseVisualStyleBackColor = true;
            
            // AIAssistantForm
            this.ClientSize = new System.Drawing.Size(636, 386);
            this.Controls.Add(this.languageComboBox);
            this.Controls.Add(this.taskTypeComboBox);
            this.Controls.Add(this.userInputTextBox);
            this.Controls.Add(this.submitButton);
            this.Controls.Add(this.copyButton);
            this.Controls.Add(this.aiOutputTextBox);
            this.Name = "AIAssistantForm";
            this.Text = "AI Assistant";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}