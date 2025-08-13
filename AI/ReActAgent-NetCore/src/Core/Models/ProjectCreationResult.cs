namespace ReActAgentNetCore.Core.Models
{
    /// <summary>
    /// 项目创建结果
    /// </summary>
    public class ProjectCreationResult
    {
        public bool Success { get; set; }
        public string? ProjectPath { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// 项目创建信息
    /// </summary>
    public class ProjectCreationInfo
    {
        public string Name { get; set; } = "";
        public string Command { get; set; } = "";
        public string Args { get; set; } = "";
        public string Description { get; set; } = "";
    }

    /// <summary>
    /// 命令执行结果
    /// </summary>
    public class CommandResult
    {
        public bool Success { get; set; }
        public string Output { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
    }

    /// <summary>
    /// 聊天消息
    /// </summary>
    public class ChatMessage
    {
        public string Role { get; set; } = "";
        public string Content { get; set; } = "";
    }
} 