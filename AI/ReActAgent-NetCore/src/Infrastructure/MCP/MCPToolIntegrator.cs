using System;
using System.Threading.Tasks;
using ReActAgentNetCore.Core.Models;
using ReActAgentNetCore.Infrastructure.ProjectCreation;

namespace ReActAgentNetCore.Infrastructure.MCP
{
    /// <summary>
    /// MCP工具集成器 - 集成Model Context Protocol工具
    /// </summary>
    public static class MCPToolIntegrator
    {
        /// <summary>
        /// 项目创建工具 - 直接通过MCP创建项目
        /// </summary>
        public static Task<ProjectCreationResult> CreateProjectViaMCP(
            string projectType, 
            string projectName, 
            string targetDirectory)
        {
            try
            {
                // 目前直接使用SmartProjectCreator，未来可以扩展为真正的MCP集成
                var creator = new SmartProjectCreator();
                return creator.CreateProjectAsync(projectType, projectName, targetDirectory);
            }
            catch (Exception ex)
            {
                return Task.FromResult(new ProjectCreationResult
                {
                    Success = false,
                    ErrorMessage = $"MCP项目创建失败: {ex.Message}"
                });
            }
        }
    }
} 