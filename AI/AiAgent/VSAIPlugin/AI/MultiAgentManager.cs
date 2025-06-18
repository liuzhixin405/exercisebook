using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Linq;

namespace VSAIPluginNew.AI
{
    public sealed class MultiAgentManager
    {
        private readonly Dictionary<string, OllamaAgent> _ollamaAgents;
        private readonly Dictionary<string, LMStudioAgent> _lmstudioAgents;
        private static readonly object _lock = new object();
        private static volatile MultiAgentManager? _instance;

        private MultiAgentManager()
        {
            _ollamaAgents = new Dictionary<string, OllamaAgent>();
            _lmstudioAgents = new Dictionary<string, LMStudioAgent>();
            
            // 初始化本地Ollama模型
            InitializeOllamaModel("qwen2.5-coder-7b", "你是一个专注于复杂代码分析和生成的AI助手", 11434);
            // 初始化LMStudio模型
            InitializeLMStudioModel("qwen/qwen3-8b", "你是一个专注于代码分析和生成的AI助手", 1234);
        }

        private void InitializeOllamaModel(string modelName, string systemMessage, int port)
        {
            try
            {
                if (!_ollamaAgents.ContainsKey(modelName))
                {
                    _ollamaAgents[modelName] = new OllamaAgent(modelName, systemMessage, port);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化模型 {modelName} 失败: {ex.Message}");
            }
        }

        private void InitializeLMStudioModel(string modelName, string systemMessage, int port)
        {
            try
            {
                if (!_lmstudioAgents.ContainsKey(modelName))
                {
                    _lmstudioAgents[modelName] = new LMStudioAgent(modelName, systemMessage, port);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化LMStudio模型 {modelName} 失败: {ex.Message}");
            }
        }

        public static MultiAgentManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new MultiAgentManager();
                    }
                }
                return _instance;
            }
        }

        public async Task<string> ProcessQueryAsync(string query, string taskType = "综合任务")
        {
            try
            {
                // 根据任务类型选择合适的模型
                var modelName = taskType switch
                {
                    "代码分析" or "代码生成" or "文本总结" => "qwen/qwen3-8b",
                    _ => "qwen2.5-coder-7b" // 默认使用大模型
                };

                if (modelName == "qwen/qwen3-8b")
                {
                    if (!_lmstudioAgents.TryGetValue(modelName, out var agent))
                    {
                        return $"错误：没有可用的LMStudio模型来处理该请求";
                    }
                    return await agent.GenerateReplyAsync(query);
                }
                else
                {
                    if (!_ollamaAgents.TryGetValue(modelName, out var agent))
                    {
                        return $"错误：没有可用的Ollama模型来处理该请求";
                    }
                    return await agent.GenerateReplyAsync(query);
                }
            }
            catch (Exception ex)
            {
                return $"处理查询时出错: {ex.Message}";
            }
        }

        public async Task<string> ProcessComplexQueryAsync(string query)
        {
            try
            {
                // 对于复杂查询，优先使用大模型
                const string modelName = "qwen2.5-coder-7b";
                if (!_ollamaAgents.TryGetValue(modelName, out var agent))
                {
                    return "错误：没有可用的Ollama大模型来处理复杂查询";
                }
                return await agent.GenerateReplyAsync(query);
            }
            catch (Exception ex)
            {
                return $"处理复杂查询时出错: {ex.Message}";
            }
        }

        public async Task<string> ProcessFilesAndGenerateReplyAsync(string query, Dictionary<string, string> filesContent)
        {
            try
            {
                // 多文件处理默认使用大模型
                const string modelName = "qwen2.5-coder-7b";
                if (!_ollamaAgents.TryGetValue(modelName, out var agent))
                {
                    throw new Exception("大模型不可用，多文件分析任务需要使用大模型进行处理。");
                }
                var prompt = $"分析以下文件并回答问题：\n\n文件内容：\n{string.Join("\n\n", filesContent)}\n\n问题：{query}";
                return await agent.GenerateReplyAsync(prompt);
            }
            catch (Exception ex)
            {
                return $"处理文件分析时出错: {ex.Message}";
            }
        }

        public void UpdateAgentSystemMessage(string modelName, string systemMessage)
        {
            if (string.IsNullOrEmpty(modelName) || string.IsNullOrEmpty(systemMessage))
            {
                throw new ArgumentException("模型名称和系统消息不能为空");
            }

            if (_ollamaAgents.TryGetValue(modelName, out var agent))
            {
                agent.UpdateSystemMessage(systemMessage);
            } 
            else
            {
                throw new ArgumentException($"未找到模型: {modelName}");
            }
        }

        public async Task<bool> IsModelAvailableAsync(string modelName)
        {
            try
            {
                if (string.IsNullOrEmpty(modelName))
                {
                    return false;
                }

                if (_ollamaAgents.TryGetValue(modelName, out var agent))
                {
                    // 使用实际的模型名称进行检查
                    var actualModelName = AgentFactory.ModelNames.TryGetValue(modelName, out string? actualName) 
                        ? actualName 
                        : modelName;
                    return await agent.IsModelAvailableAsync(actualModelName);
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
