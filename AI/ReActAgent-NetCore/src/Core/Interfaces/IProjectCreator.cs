using ReActAgentNetCore.Core.Models;

namespace ReActAgentNetCore.Core.Interfaces
{
    /// <summary>
    /// 项目创建器接口
    /// </summary>
    public interface IProjectCreator
    {
        /// <summary>
        /// 创建项目
        /// </summary>
        /// <param name="projectType">项目类型</param>
        /// <param name="projectName">项目名称</param>
        /// <param name="targetDirectory">目标目录</param>
        /// <returns>项目创建结果</returns>
        Task<ProjectCreationResult> CreateProjectAsync(string projectType, string projectName, string targetDirectory);
    }

    /// <summary>
    /// 智能项目创建器接口
    /// </summary>
    public interface ISmartProjectCreator : IProjectCreator
    {
        /// <summary>
        /// 分析用户需求并推荐项目类型
        /// </summary>
        /// <param name="userRequest">用户请求</param>
        /// <returns>推荐的项目类型列表</returns>
        List<ProjectCreationInfo> AnalyzeUserRequest(string userRequest);

        /// <summary>
        /// 获取所有支持的项目类型
        /// </summary>
        /// <returns>支持的项目类型列表</returns>
        List<ProjectCreationInfo> GetAllProjectTypes();
    }
} 