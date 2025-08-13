using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using ReActAgentNetCore.Core.Models;
using ReActAgentNetCore.Core.Interfaces;

namespace ReActAgentNetCore.Infrastructure.ProjectCreation
{
    /// <summary>
    /// 命令行项目创建器 - 通过官方CLI工具快速创建项目
    /// </summary>
    public class CLIProjectCreator : IProjectCreator
    {
        /// <summary>
        /// 创建项目（实现接口方法）
        /// </summary>
        public async Task<ProjectCreationResult> CreateProjectAsync(string projectType, string projectName, string targetDirectory)
        {
            // 统一别名映射
            var lower = projectType.ToLower();
            if (lower == "solidity" || lower == "smart-contract" || lower == "contract" || lower == "ethereum" || lower == "web3")
            {
                // 将别名统一映射为默认的 hardhat 模板
                return await CreateBlockchainProjectAsync("hardhat", projectName, targetDirectory);
            }

            // 根据项目类型调用相应的方法
            if (projectType.StartsWith("dotnet-"))
            {
                var actualType = projectType.Replace("dotnet-", "");
                return await CreateDotNetProjectAsync(actualType, projectName, targetDirectory);
            }
            else if (new[] { "react", "vue", "express", "next", "nuxt" }.Contains(projectType.ToLower()))
            {
                return await CreateNodeProjectAsync(projectType, projectName, targetDirectory);
            }
            else if (new[] { "flask", "django", "fastapi", "streamlit" }.Contains(projectType.ToLower()))
            {
                return await CreatePythonProjectAsync(projectType, projectName, targetDirectory);
            }
            else if (new[] { "hardhat", "foundry", "truffle" }.Contains(projectType.ToLower()))
            {
                return await CreateBlockchainProjectAsync(projectType, projectName, targetDirectory);
            }
            else if (new[] { "react-native", "expo", "flutter" }.Contains(projectType.ToLower()))
            {
                return await CreateMobileProjectAsync(projectType, projectName, targetDirectory);
            }
            else
            {
                return new ProjectCreationResult
                {
                    Success = false,
                    ErrorMessage = $"不支持的项目类型: {projectType}"
                };
            }
        }

        /// <summary>
        /// 快速创建.NET项目
        /// </summary>
        public async Task<ProjectCreationResult> CreateDotNetProjectAsync(
            string projectType, 
            string projectName, 
            string targetDirectory)
        {
            string command, args;

            switch (projectType.ToLower())
            {
                case "webapp":
                    command = "dotnet";
                    args = $"new webapp -n {projectName} -o {projectName}";
                    break;
                case "webapi":
                    command = "dotnet";
                    args = $"new webapi -n {projectName} -o {projectName}";
                    break;
                case "console":
                    command = "dotnet";
                    args = $"new console -n {projectName} -o {projectName}";
                    break;
                case "classlib":
                    command = "dotnet";
                    args = $"new classlib -n {projectName} -o {projectName}";
                    break;
                case "mvc":
                    command = "dotnet";
                    args = $"new mvc -n {projectName} -o {projectName}";
                    break;
                case "blazorserver":
                    command = "dotnet";
                    args = $"new blazorserver -n {projectName} -o {projectName}";
                    break;
                case "blazorwasm":
                    command = "dotnet";
                    args = $"new blazorwasm -n {projectName} -o {projectName}";
                    break;
                default:
                    return new ProjectCreationResult
                    {
                        Success = false,
                        ErrorMessage = $"不支持的.NET项目类型: {projectType}"
                    };
            }

            return await ExecuteCommandAsync(command, args, targetDirectory, projectName);
        }

        /// <summary>
        /// 快速创建Node项目
        /// </summary>
        public async Task<ProjectCreationResult> CreateNodeProjectAsync(
            string projectType, 
            string projectName, 
            string targetDirectory)
        {
            string command, args;

            switch (projectType.ToLower())
            {
                case "react":
                    command = "npx";
                    args = $"create-react-app {projectName}";
                    break;
                case "vue":
                    command = "npm";
                    args = $"create vue@latest {projectName}";
                    break;
                case "express":
                    command = "npx";
                    args = $"express-generator {projectName}";
                    break;
                case "next":
                    command = "npx";
                    args = $"create-next-app@latest {projectName}";
                    break;
                case "nuxt":
                    command = "npx";
                    args = $"nuxi@latest init {projectName}";
                    break;
                default:
                    return new ProjectCreationResult
                    {
                        Success = false,
                        ErrorMessage = $"不支持的Node项目类型: {projectType}"
                    };
            }

            return await ExecuteCommandAsync(command, args, targetDirectory, projectName);
        }

        /// <summary>
        /// 快速创建Python项目
        /// </summary>
        public async Task<ProjectCreationResult> CreatePythonProjectAsync(
            string projectType, 
            string projectName, 
            string targetDirectory)
        {
            string command, args;

            switch (projectType.ToLower())
            {
                case "flask":
                    command = "python";
                    args = $"-m flask startproject {projectName}";
                    break;
                case "django":
                    command = "django-admin";
                    args = $"startproject {projectName}";
                    break;
                case "fastapi":
                    command = "uvx";
                    args = $"--from fastapi-cli fastapi startproject {projectName}";
                    break;
                case "streamlit":
                    command = "streamlit";
                    args = $"hello --server.headless true";
                    // Streamlit需要特殊处理
                    return await CreateStreamlitProjectAsync(projectName, targetDirectory);
                default:
                    return new ProjectCreationResult
                    {
                        Success = false,
                        ErrorMessage = $"不支持的Python项目类型: {projectType}"
                    };
            }

            return await ExecuteCommandAsync(command, args, targetDirectory, projectName);
        }

        /// <summary>
        /// 快速创建区块链项目
        /// </summary>
        public async Task<ProjectCreationResult> CreateBlockchainProjectAsync(
            string projectType, 
            string projectName, 
            string targetDirectory)
        {
            string command, args;

            // 别名映射：将 "solidity" 等统一映射为默认的 hardhat
            var type = projectType.ToLower();
            if (type == "solidity" || type == "smart-contract" || type == "contract" || type == "ethereum" || type == "web3")
            {
                type = "hardhat";
            }

            switch (type)
            {
                case "hardhat":
                    command = "npx";
                    args = $"hardhat@latest init {projectName}";
                    break;
                case "foundry":
                    command = "forge";
                    args = $"init {projectName}";
                    break;
                case "truffle":
                    command = "npx";
                    args = $"truffle@latest init {projectName}";
                    break;
                default:
                    return new ProjectCreationResult
                    {
                        Success = false,
                        ErrorMessage = $"不支持的区块链项目类型: {projectType}"
                    };
            }

            return await ExecuteCommandAsync(command, args, targetDirectory, projectName);
        }

        /// <summary>
        /// 快速创建移动应用项目
        /// </summary>
        public async Task<ProjectCreationResult> CreateMobileProjectAsync(
            string projectType, 
            string projectName, 
            string targetDirectory)
        {
            string command, args;

            switch (projectType.ToLower())
            {
                case "react-native":
                    command = "npx";
                    args = $"react-native@latest init {projectName}";
                    break;
                case "expo":
                    command = "npx";
                    args = $"create-expo-app@latest {projectName}";
                    break;
                case "flutter":
                    command = "flutter";
                    args = $"create {projectName}";
                    break;
                default:
                    return new ProjectCreationResult
                    {
                        Success = false,
                        ErrorMessage = $"不支持的移动应用项目类型: {projectType}"
                    };
            }

            return await ExecuteCommandAsync(command, args, targetDirectory, projectName);
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        private async Task<ProjectCreationResult> ExecuteCommandAsync(
            string command, 
            string arguments, 
            string workingDirectory, 
            string projectName)
        {
            try
            {
                var projectPath = Path.Combine(workingDirectory, projectName);
                
                if (Directory.Exists(projectPath))
                {
                    return new ProjectCreationResult
                    {
                        Success = false,
                        ErrorMessage = $"项目目录已存在: {projectPath}"
                    };
                }

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

                if (process.ExitCode == 0)
                {
                    return new ProjectCreationResult
                    {
                        Success = true,
                        Message = output.ToString(),
                        ProjectPath = Path.Combine(workingDirectory, projectName)
                    };
                }
                else
                {
                    return new ProjectCreationResult
                    {
                        Success = false,
                        ErrorMessage = error.ToString()
                    };
                }
            }
            catch (Win32Exception ex)
            {
                // 可执行文件不可用时的回退（例如未安装 npx/Node.js）
                if (command.Equals("npx", StringComparison.OrdinalIgnoreCase) && arguments.Contains("hardhat", StringComparison.OrdinalIgnoreCase))
                {
                    return await CreateHardhatScaffoldAsync(projectName, workingDirectory, ex.Message);
                }
                return new ProjectCreationResult
                {
                    Success = false,
                    ErrorMessage = $"无法启动命令 '{command}': {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ProjectCreationResult
                {
                    Success = false,
                    ErrorMessage = $"执行命令时发生异常: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 为 Streamlit 创建示例项目（特殊处理）
        /// </summary>
        private async Task<ProjectCreationResult> CreateStreamlitProjectAsync(string projectName, string targetDirectory)
        {
            try
            {
                var projectPath = Path.Combine(targetDirectory, projectName);
                Directory.CreateDirectory(projectPath);
                var appPyPath = Path.Combine(projectPath, "app.py");
                await File.WriteAllTextAsync(appPyPath, "import streamlit as st\n\nst.title('Hello, Streamlit!')\n\nst.write('This is a demo app.')\n");
                return new ProjectCreationResult
                {
                    Success = true,
                    Message = "已创建 Streamlit 示例项目",
                    ProjectPath = projectPath
                };
            }
            catch (Exception ex)
            {
                return new ProjectCreationResult
                {
                    Success = false,
                    ErrorMessage = $"创建 Streamlit 项目失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 当 npx 不可用时，创建一个最小可用的 Hardhat 项目脚手架
        /// </summary>
        private async Task<ProjectCreationResult> CreateHardhatScaffoldAsync(string projectName, string workingDirectory, string reason)
        {
            try
            {
                var projectPath = Path.Combine(workingDirectory, projectName);
                Directory.CreateDirectory(projectPath);
                Directory.CreateDirectory(Path.Combine(projectPath, "contracts"));
                Directory.CreateDirectory(Path.Combine(projectPath, "scripts"));
                Directory.CreateDirectory(Path.Combine(projectPath, "test"));

                var packageJson = "{\n  \"name\": \"" + projectName + "\",\n  \"private\": true,\n  \"devDependencies\": {\n    \"hardhat\": \"^2.22.0\"\n  }\n}\n";
                var hardhatConfig = "require(\"@nomicfoundation/hardhat-toolbox\");\n\nmodule.exports = {\n  solidity: \"0.8.20\"\n};\n";
                var sampleContract = "// SPDX-License-Identifier: MIT\npragma solidity ^0.8.20;\n\ncontract MyContract {\n    string public message;\n\n    constructor(string memory initialMessage) {\n        message = initialMessage;\n    }\n\n    function setMessage(string memory newMessage) public {\n        message = newMessage;\n    }\n}\n";
                var readme = $"# {projectName}\n\n这是一个最小可用的 Hardhat 项目脚手架（由于本机缺少 npx/Node.js，已自动回退生成）。\n\n## 使用步骤\n1. 安装 Node.js 与 npm\n2. 在本目录执行: `npm install --save-dev hardhat @nomicfoundation/hardhat-toolbox`\n3. 初始化 Hardhat（可选）: `npx hardhat`\n4. 编译合约: `npx hardhat compile`\n\n## 结构\n- contracts/MyContract.sol\n- hardhat.config.js\n- package.json\n- scripts/\n- test/\n\n## 说明\n自动回退原因: {reason}\n";

                await File.WriteAllTextAsync(Path.Combine(projectPath, "package.json"), packageJson, Encoding.UTF8);
                await File.WriteAllTextAsync(Path.Combine(projectPath, "hardhat.config.js"), hardhatConfig, Encoding.UTF8);
                await File.WriteAllTextAsync(Path.Combine(projectPath, "contracts", "MyContract.sol"), sampleContract, Encoding.UTF8);
                await File.WriteAllTextAsync(Path.Combine(projectPath, "README.md"), readme, Encoding.UTF8);

                return new ProjectCreationResult
                {
                    Success = true,
                    Message = $"已创建最小 Hardhat 项目脚手架（未执行依赖安装）。\n路径: {projectPath}",
                    ProjectPath = projectPath
                };
            }
            catch (Exception ex)
            {
                return new ProjectCreationResult
                {
                    Success = false,
                    ErrorMessage = $"创建回退脚手架失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 获取支持的项目类型列表
        /// </summary>
        public Dictionary<string, List<string>> GetSupportedProjectTypes()
        {
            return new Dictionary<string, List<string>>
            {
                ["dotnet"] = new List<string> { "webapp", "webapi", "console", "classlib", "mvc", "blazorserver", "blazorwasm" },
                ["node"] = new List<string> { "react", "vue", "express", "next", "nuxt", "svelte" },
                ["python"] = new List<string> { "flask", "django", "fastapi", "streamlit" },
                ["blockchain"] = new List<string> { "hardhat", "foundry", "truffle" },
                ["mobile"] = new List<string> { "react-native", "expo", "flutter" }
            };
        }
    }
} 