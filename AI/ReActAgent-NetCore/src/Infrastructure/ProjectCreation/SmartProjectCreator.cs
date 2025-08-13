using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReActAgentNetCore.Core.Models;
using ReActAgentNetCore.Core.Interfaces;

namespace ReActAgentNetCore.Infrastructure.ProjectCreation
{
    /// <summary>
    /// 智能项目创建器 - 通过官方CLI工具创建项目
    /// </summary>
    public class SmartProjectCreator : ISmartProjectCreator
    {
        /// <summary>
        /// 支持的项目类型和对应的创建命令
        /// </summary>
        private static readonly Dictionary<string, ProjectCreationInfo> ProjectTypes = new()
        {
            // .NET 项目
            ["dotnet-web"] = new ProjectCreationInfo
            {
                Name = "ASP.NET Core Web App",
                Command = "dotnet",
                Args = "new webapp -n {projectName} -o {projectName}",
                Description = "创建ASP.NET Core Web应用"
            },
            ["dotnet-api"] = new ProjectCreationInfo
            {
                Name = "ASP.NET Core Web API",
                Command = "dotnet",
                Args = "new webapi -n {projectName} -o {projectName}",
                Description = "创建ASP.NET Core Web API"
            },
            ["react"] = new ProjectCreationInfo
            {
                Name = "React App",
                Command = "npx",
                Args = "create-react-app {projectName}",
                Description = "创建React应用"
            },
            ["vue"] = new ProjectCreationInfo
            {
                Name = "Vue App",
                Command = "npm",
                Args = "create vue@latest {projectName}",
                Description = "创建Vue应用"
            },
            ["python-flask"] = new ProjectCreationInfo
            {
                Name = "Flask App",
                Command = "python",
                Args = "-m flask startproject {projectName}",
                Description = "创建Flask应用"
            },
            ["hardhat"] = new ProjectCreationInfo
            {
                Name = "Hardhat Project",
                Command = "npx",
                Args = "hardhat@latest init {projectName}",
                Description = "创建Hardhat区块链项目"
            }
        };

        /// <summary>
        /// 分析用户需求并推荐项目类型
        /// </summary>
        public List<ProjectCreationInfo> AnalyzeUserRequest(string userRequest)
        {
            var requestLower = userRequest.ToLower();
            var recommendations = new List<ProjectCreationInfo>();

            foreach (var projectType in ProjectTypes)
            {
                var score = CalculateRelevanceScore(requestLower, projectType.Value);
                if (score > 0.3)
                {
                    recommendations.Add(projectType.Value);
                }
            }

            recommendations.Sort((a, b) => 
                CalculateRelevanceScore(requestLower, b).CompareTo(
                    CalculateRelevanceScore(requestLower, a)));

            return recommendations.Take(5).ToList();
        }

        /// <summary>
        /// 计算相关性分数
        /// </summary>
        private double CalculateRelevanceScore(string userRequest, ProjectCreationInfo projectInfo)
        {
            var score = 0.0;
            var keywords = new List<string>();

            if (projectInfo.Name.Contains("React")) keywords.AddRange(new[] { "react", "前端", "ui", "web" });
            if (projectInfo.Name.Contains("Vue")) keywords.AddRange(new[] { "vue", "前端", "ui", "web" });
            if (projectInfo.Name.Contains("API")) keywords.AddRange(new[] { "api", "后端", "服务", "rest" });
            if (projectInfo.Name.Contains("Python")) keywords.AddRange(new[] { "python", "py", "脚本" });
            if (projectInfo.Name.Contains("Hardhat")) keywords.AddRange(new[] { "区块链", "智能合约", "solidity", "web3" });

            foreach (var keyword in keywords)
            {
                if (userRequest.Contains(keyword))
                {
                    score += 0.2;
                }
            }

            return Math.Min(score, 1.0);
        }

        /// <summary>
        /// 创建项目
        /// </summary>
        public async Task<ProjectCreationResult> CreateProjectAsync(
            string projectType, 
            string projectName, 
            string targetDirectory)
        {
            if (!ProjectTypes.ContainsKey(projectType))
            {
                return new ProjectCreationResult
                {
                    Success = false,
                    ErrorMessage = $"不支持的项目类型: {projectType}"
                };
            }

            var projectInfo = ProjectTypes[projectType];
            var fullPath = Path.Combine(targetDirectory, projectName);

            try
            {
                if (Directory.Exists(fullPath))
                {
                    return new ProjectCreationResult
                    {
                        Success = false,
                        ErrorMessage = $"项目目录已存在: {fullPath}"
                    };
                }

                var args = projectInfo.Args.Replace("{projectName}", projectName);
                var result = await ExecuteCommandAsync(projectInfo.Command, args, targetDirectory);

                if (result.Success)
                {
                    return new ProjectCreationResult
                    {
                        Success = true,
                        ProjectPath = fullPath,
                        Message = $"项目 {projectName} 创建成功！"
                    };
                }
                else
                {
                    return new ProjectCreationResult
                    {
                        Success = false,
                        ErrorMessage = $"创建项目失败: {result.ErrorMessage}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ProjectCreationResult
                {
                    Success = false,
                    ErrorMessage = $"创建项目时发生异常: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        private async Task<CommandResult> ExecuteCommandAsync(string command, string arguments, string workingDirectory)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = startInfo };
                var output = new StringBuilder();
                var error = new StringBuilder();

                process.OutputDataReceived += (sender, e) => 
                {
                    if (e.Data != null) output.AppendLine(e.Data);
                };
                process.ErrorDataReceived += (sender, e) => 
                {
                    if (e.Data != null) error.AppendLine(e.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();

                return new CommandResult
                {
                    Success = process.ExitCode == 0,
                    Output = output.ToString(),
                    ErrorMessage = error.ToString()
                };
            }
            catch (Exception ex)
            {
                return new CommandResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// 获取所有支持的项目类型
        /// </summary>
        public List<ProjectCreationInfo> GetAllProjectTypes()
        {
            return ProjectTypes.Values.ToList();
        }
    }
} 