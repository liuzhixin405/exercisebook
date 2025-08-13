using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ReActAgentNetCore.Core.Models;
using ReActAgentNetCore.Infrastructure.Tools;

namespace ReActAgentNetCore.Application.Agents
{
    public class ReActAgent
    {
        private readonly Dictionary<string, Delegate> _tools;
        private readonly string _model;
        private readonly string _projectDirectory;
        private readonly HttpClient _httpClient;
        private readonly string _ollamaBaseUrl;

        public ReActAgent(Dictionary<string, Delegate> tools, string model, string projectDirectory, string ollamaBaseUrl = "http://localhost:11434")
        {
            _tools = tools;
            _model = model;
            _projectDirectory = projectDirectory;
            _ollamaBaseUrl = ollamaBaseUrl;
            _httpClient = new HttpClient();
        }

        public async Task<string> RunAsync(string userInput)
        {
            var messages = new List<ChatMessage>
            {
                new ChatMessage { Role = "system", Content = RenderSystemPrompt() },
                new ChatMessage { Role = "user", Content = $"<question>{userInput}</question>" }
            };

            // 添加迭代计数器防止无限循环
            int iterationCount = 0;
            const int maxIterations = 20; // 最大迭代次数

            while (iterationCount < maxIterations)
            {
                iterationCount++;
                Console.WriteLine($"\n🔄 迭代次数: {iterationCount}/{maxIterations}");

                // 调用模型
                var content = await CallModelAsync(messages);

                // 提取Thought
                var thoughtMatch = Regex.Match(content, @"<thought>(.*?)</thought>", RegexOptions.Singleline);
                if (thoughtMatch.Success)
                {
                    var thought = thoughtMatch.Groups[1].Value;
                    Console.WriteLine($"\n\n🤔 Thought: {thought}");
                }

                // 提取Final Answer
                if (content.Contains("<final_answer>"))
                {
                    var finalAnswerMatch = Regex.Match(content, @"<final_answer>(.*?)</final_answer>", RegexOptions.Singleline);
                    return finalAnswerMatch.Groups[1].Value;
                }

                // 提取Action
                var actionMatch = Regex.Match(content, @"<action>(.*?)</action>", RegexOptions.Singleline);
                if (!actionMatch.Success)
                {
                    Console.WriteLine($"\n错误：未找到 <action> 标签\n完整响应：{content}");
                    Console.WriteLine($"\n请确保模型返回包含 <action> 标签的响应");
                    throw new InvalidOperationException($"未找到 <action> 标签！完整响应：{content}");
                }

                var action = actionMatch.Groups[1].Value;
                var (toolName, args) = ParseAction(action);

                Console.WriteLine($"\n\n🛠️ Action: {toolName}({string.Join(", ", args)})");

                // 安全检查：对于终端命令需要用户确认
                bool shouldContinue = true;
                if (toolName == "run_terminal_command")
                {
                    Console.Write("\n\n⚠️ 检测到终端命令，是否继续执行？(Y/N): ");
                    var input = Console.ReadLine();
                    shouldContinue = input?.ToLower() == "y";
                }

                if (!shouldContinue)
                {
                    Console.WriteLine("\n\n🛑 用户取消操作");
                    return "用户取消操作";
                }

                try
                {
                    var observation = await ExecuteToolAsync(toolName, args);
                    Console.WriteLine($"\n\n🔍 Observation:\n{observation}");
                    var obsMsg = $"<observation>{observation}</observation>";
                    messages.Add(new ChatMessage { Role = "user", Content = obsMsg });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n\n❌ 工具执行失败：{ex.Message}");
                    var errorMsg = $"<error>工具执行失败：{ex.Message}</error>";
                    messages.Add(new ChatMessage { Role = "user", Content = errorMsg });
                }
            }

            return "达到最大迭代次数，任务未完成";
        }

        /// <summary>
        /// 调用Ollama模型
        /// </summary>
        private async Task<string> CallModelAsync(List<ChatMessage> messages)
        {
            try
            {
                var request = new
                {
                    model = _model,
                    messages = messages.Select(m => new { role = m.Role, content = m.Content }).ToArray(),
                    stream = false
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_ollamaBaseUrl}/api/chat", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseObj = JsonSerializer.Deserialize<JsonElement>(responseContent);

                return responseObj.GetProperty("message").GetProperty("content").GetString() ?? "";
            }
            catch (Exception ex)
            {
                throw new Exception($"调用Ollama模型失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 解析Action标签
        /// </summary>
        private (string toolName, string[] args) ParseAction(string action)
        {
            var match = Regex.Match(action, @"(\w+)\(([^)]*)\)");
            if (!match.Success)
            {
                throw new ArgumentException($"无法解析Action：{action}");
            }

            var toolName = match.Groups[1].Value;
            var argsStr = match.Groups[2].Value;

            var args = argsStr.Split(',')
                .Select(arg => arg.Trim().Trim('"', '\''))
                .Where(arg => !string.IsNullOrEmpty(arg))
                .ToArray();

            return (toolName, args);
        }

        /// <summary>
        /// 执行工具
        /// </summary>
        private async Task<string> ExecuteToolAsync(string toolName, string[] args)
        {
            if (!_tools.ContainsKey(toolName))
            {
                throw new ArgumentException($"未知的工具：{toolName}");
            }

            var tool = _tools[toolName];
            var parameters = tool.Method.GetParameters();

            if (parameters.Length != args.Length)
            {
                throw new ArgumentException($"工具 {toolName} 需要 {parameters.Length} 个参数，但提供了 {args.Length} 个");
            }

            // 转换参数类型
            var convertedArgs = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                var paramType = parameters[i].ParameterType;
                if (paramType == typeof(string))
                {
                    convertedArgs[i] = args[i];
                }
                else if (paramType == typeof(int))
                {
                    convertedArgs[i] = int.Parse(args[i]);
                }
                else if (paramType == typeof(bool))
                {
                    convertedArgs[i] = bool.Parse(args[i]);
                }
                else
                {
                    throw new ArgumentException($"不支持的参数类型：{paramType.Name}");
                }
            }

            var result = tool.DynamicInvoke(convertedArgs);
            if (result is Task<string> task)
            {
                return await task;
            }
            else if (result is string str)
            {
                return str;
            }
            else
            {
                return result?.ToString() ?? "";
            }
        }

        /// <summary>
        /// 渲染系统提示
        /// </summary>
        private string RenderSystemPrompt()
        {
            return $@"你是一个智能编程助手，能够帮助用户完成各种编程任务。

可用工具：
- read_file(filePath): 读取文件内容
- write_to_file(filePath, content): 写入文件内容
- run_terminal_command(command): 执行终端命令
- list_directory(directoryPath): 列出目录内容
- search_files(searchPattern, directoryPath): 搜索文件
- create_directory(directoryPath): 创建目录
- create_dotnet_project(projectType, projectName): 创建.NET项目
- create_node_project(projectType, projectName): 创建Node.js项目
- create_python_project(projectType, projectName): 创建Python项目
- create_blockchain_project(projectType, projectName): 创建区块链项目
- create_mobile_project(projectType, projectName): 创建移动应用项目
- create_project_via_mcp(projectType, projectName): 通过MCP创建项目

工作目录：{_projectDirectory}

使用说明：
1. 仔细分析用户需求
2. 选择合适的工具完成任务
3. 优先使用智能项目创建工具，减少手动创建文件
4. 对于复杂任务，可以分步骤执行
5. 确保所有操作都在正确的工作目录下进行

智能项目创建指南：
1. 对于常见项目类型，优先使用 create_dotnet_project、create_node_project、create_python_project 等工具
2. 这些工具会自动处理项目结构和依赖
3. 用户需要创建一个React项目，我应该使用智能项目创建工具而不是手动创建文件

示例：
用户需要创建一个React项目，我应该使用智能项目创建工具
<action>create_node_project(""react"", ""my-react-app"")</action>

用户需要创建一个.NET Web API项目，我应该使用智能项目创建工具
<action>create_dotnet_project(""webapi"", ""my-api"")</action>

用户需要创建一个Python Flask项目，我应该使用智能项目创建工具
<action>create_python_project(""flask"", ""my-flask-app"")</action>

请按照以下格式回复：
<thought>分析用户需求，确定需要使用的工具</thought>
<action>tool_name(""arg1"", ""arg2"")</action>

如果任务完成，请使用：
<final_answer>任务完成，结果说明</final_answer>";
        }
    }
} 