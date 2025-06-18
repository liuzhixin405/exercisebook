#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace VSAIPluginNew.AI
{
    public class AgentFactory
    {
        private static readonly object _lock = new object();
        
        // Ollama模型列表
        public static readonly List<string> OllamaModels = new List<string>
        {
            "qwen2.5-coder-7b",  // 大模型，处理复杂任务
            "qwen/qwen3-8b"        // 小模型，全部用KMStudio的qwen/qwen3-8b
        };

        // 模型端口映射
        public static readonly Dictionary<string, int> ModelPorts = new Dictionary<string, int>
        {
            ["qwen2.5-coder-7b"] = 11434,
            ["qwen/qwen3-8b"] = 1234 // 假设KMStudio端口为1234，如有不同请调整
        };

        // Ollama模型实际名称映射
        public static readonly Dictionary<string, string> ModelNames = new Dictionary<string, string>
        {
            ["qwen2.5-coder-7b"] = "qwen2.5-coder:7b",
            ["qwen/qwen3-8b"] = "qwen/qwen3-8b"
        };

        // 模型任务分配映射
        public static readonly Dictionary<string, string> TaskModelMapping = new Dictionary<string, string>
        {
            ["代码分析"] = "qwen/qwen3-8b",        // 简单的代码分析任务
            ["代码生成"] = "qwen/qwen3-8b",        // 简单的代码生成任务
            ["文本总结"] = "qwen/qwen3-8b",        // 简单的文本处理任务
            ["复杂分析"] = "qwen2.5-coder-7b",     // 复杂的代码分析任务
            ["多文件处理"] = "qwen2.5-coder-7b",   // 涉及多个文件的任务
            ["架构设计"] = "qwen2.5-coder-7b",     // 系统架构相关任务
            ["综合任务"] = "qwen2.5-coder-7b"      // 默认使用大模型
        };
        
        // 定义一组预设的系统消息模板
        public static readonly Dictionary<string, string> SystemMessageTemplates = new Dictionary<string, string>
        {
            ["聊天"] = @"你是Visual Studio 2022的AI助手。
你应该简单直接地回答用户的问题。
不需要执行复杂的代码分析或任务。",
            
            ["代理"] = @"你是Visual Studio 2022的AI代理助手。
你能够执行复杂的代码分析和任务。

【重要路径规则】:
1. 始终使用相对路径，不要使用绝对路径（如D:\或C:\）
2. 使用简单相对路径，例如'Models/User.cs'或'Controllers/HomeController.cs'

当建议代码更改时，请使用以下格式标记代码块：
// 完整的代码内容(不是片段)
或者
// 完整的代码内容(不是片段)
如果需要创建新文件或目录，明确指出：

创建文件: Models/Product.cs// 代码内容
创建目录: Data

如果需要修改现有文件：

修改文件: Controllers/HomeController.cs// 修改后的完整代码内容
当需要运行命令时，使用以下格式：

命令: cmd /c 命令内容
或
命令: powershell -Command 命令内容
或
命令: dotnet 命令内容

【操作顺序】:
如果需要执行多个操作，请按正确顺序列出：
1. 先执行命令（如dotnet new console）
2. 然后创建或修改文件

请确保按照上述格式提供代码和命令，这样系统能正确解析和执行。",

            ["MCP增强"] = @"你是Visual Studio 2022的MCP增强型AI助手。
你能够执行复杂的代码分析和文件系统操作。

【工作流程】:
1. **分析请求**: 理解用户的目标。
2. **收集信息 (如果需要)**: 如果需要项目结构或文件内容信息才能回答，请使用下面的MCP工具来获取。
3. **制定计划 (如果需要)**: 对于复杂任务，使用【顺序思考】工具来制定步骤。
4. **执行操作 (如果请求)**: 如果用户要求创建/修改文件或运行命令，请生成相应的指令。
5. **生成回复**: 基于收集的信息和执行的操作给出最终答复。

【MCP工具用法】:
*   **理解项目结构**: 使用 `【列出目录】路径` (例如 `【列出目录】.` 或 `【列出目录】Services`)
*   **查找文件**: 使用 `【搜索文件】模式 [目录]` (例如 `【搜索文件】*Controller.cs Controllers`)
*   **读取文件**: 使用 `【读取文件】文件路径` (例如 `【读取文件】Services/UserService.cs`). **不要假设文件内容，请先读取它！**
*   **写入/创建文件**: 生成 `创建文件: 文件路径` 或 `修改文件: 文件路径` 指令，后跟代码块：```文件:Services/UserService.cs
    // 完整代码内容*   **执行命令**: 生成 `执行命令: 命令内容` 指令 (例如 `执行命令: dotnet build`)
*   **获取网页**: 使用 `【获取网页】URL`
*   **复杂规划**: 使用 `【顺序思考】问题 [步数]`

【重要提示】：
1. **优先使用工具获取信息**，特别是文件内容。
2. 始终使用**相对路径**。
3. 对于代码修改或创建，必须提供**完整的代码块**。
4. 清晰地生成工具调用或操作指令。"
        };

        public static async Task UpdateAgentAsync(string modelName, string templateKey)
        {
            var systemMessage = (templateKey != null && SystemMessageTemplates.ContainsKey(templateKey)) 
                ? SystemMessageTemplates[templateKey] 
                : SystemMessageTemplates["代理"];
            
            if (OllamaModels.Contains(modelName))
            {
                try
                {
                    // 更新Ollama模型的系统消息
                    var manager = GetMultiAgentManager();
                    manager.UpdateAgentSystemMessage(modelName, systemMessage);
                    
                    // 检查模型是否可用
                    var isAvailable = await manager.IsModelAvailableAsync(modelName);
                    if (!isAvailable)
                    {
                        throw new Exception($"模型 {modelName} 不可用，请检查Ollama服务是否正常运行。");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"更新模型 {modelName} 失败: {ex.Message}");
                }
            }
        }
        
        // 获取特定模型是否可用
        public static async Task<bool> IsModelAvailableAsync(string modelName)
        {
            try
            {
                if (OllamaModels.Contains(modelName))
                {
                    return await MultiAgentManager.Instance.IsModelAvailableAsync(modelName);
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        
        // 处理复杂查询
        public static async Task<string> ProcessComplexQueryAsync(string query, string taskType = "综合任务")
        {
            try
            {
                var modelName = "qwen2.5-coder-7b"; // 默认使用大模型
                if (TaskModelMapping.ContainsKey(taskType))
                {
                    modelName = TaskModelMapping[taskType];
                }

                var manager = GetMultiAgentManager();
                
                // 检查指定模型是否可用
                if (!await manager.IsModelAvailableAsync(modelName))
                {
                    // 如果指定模型不可用，尝试使用另一个模型
                    var fallbackModel = modelName == "qwen2.5-coder-7b" ? "qwen/qwen3-8b" : "qwen2.5-coder-7b";
                    if (await manager.IsModelAvailableAsync(fallbackModel))
                    {
                        modelName = fallbackModel;
                    }
                    else
                    {
                        throw new Exception("所有模型都不可用，请检查服务是否正常运行。");
                    }
                }

                return await manager.ProcessComplexQueryAsync(query);
            }
            catch (Exception ex)
            {
                return $"处理查询时出错: {ex.Message}";
            }
        }

        public static async Task<string> ProcessFilesAsync(string query, Dictionary<string, string> filesContent)
        {
            try
            {
                var manager = GetMultiAgentManager();
                // 多文件处理默认使用大模型
                const string modelName = "qwen2.5-coder-7b";
                
                if (!await manager.IsModelAvailableAsync(modelName))
                {
                    throw new Exception("大模型不可用，多文件分析任务需要使用大模型进行处理。");
                }
                
                return await manager.ProcessFilesAndGenerateReplyAsync(query, filesContent);
            }
            catch (Exception ex)
            {
                return $"处理文件时出错: {ex.Message}";
            }
        }

        public static async Task<bool> CheckAllModelsAvailableAsync()
        {
            try
            {
                var manager = GetMultiAgentManager();
                foreach (var modelName in OllamaModels)
                {
                    var isAvailable = await manager.IsModelAvailableAsync(modelName);
                    if (!isAvailable)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static MultiAgentManager GetMultiAgentManager()
        {
            return MultiAgentManager.Instance;
        }
    }
}