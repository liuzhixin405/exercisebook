namespace ReActAgentNetCore.Core.Interfaces
{
    /// <summary>
    /// 文件操作工具接口
    /// </summary>
    public interface IFileTools
    {
        Task<string> ReadFileAsync(string filePath);
        Task<string> WriteToFileAsync(string filePath, string content);
    }

    /// <summary>
    /// 目录操作工具接口
    /// </summary>
    public interface IDirectoryTools
    {
        Task<string> ListDirectoryAsync(string directoryPath);
        Task<string> CreateDirectoryAsync(string directoryPath);
        Task<string> SearchFilesAsync(string searchPattern, string directoryPath = "");
    }

    /// <summary>
    /// 终端命令工具接口
    /// </summary>
    public interface ITerminalTools
    {
        Task<string> RunTerminalCommandAsync(string command);
    }

    /// <summary>
    /// 项目创建工具接口
    /// </summary>
    public interface IProjectCreationTools
    {
        Task<string> CreateDotNetProjectAsync(string projectType, string projectName);
        Task<string> CreateNodeProjectAsync(string projectType, string projectName);
        Task<string> CreatePythonProjectAsync(string projectType, string projectName);
        Task<string> CreateBlockchainProjectAsync(string projectType, string projectName);
        Task<string> CreateMobileProjectAsync(string projectType, string projectName);
        Task<string> CreateProjectViaMCPAsync(string projectType, string projectName);
    }
} 