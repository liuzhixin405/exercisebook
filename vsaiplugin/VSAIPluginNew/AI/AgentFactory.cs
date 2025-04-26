using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace VSAIPluginNew.AI
{
    public class AgentFactory
    {
        private static OllamaAgent? _instance;
        private static readonly object _lock = new object();

        public static OllamaAgent GetOllamaAgent()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new OllamaAgent(
                            modelName: "llama3", // 默认模型
                            systemMessage: @"You are a helpful AI assistant for Visual Studio 2022. 
You can read code files and help the developer understand, modify, and debug their code.
When suggesting code changes, be specific about which file and lines need to be changed.
Always format code blocks with the appropriate language syntax highlighting."
                        );
                    }
                }
            }
            return _instance;
        }

        // 检查Ollama是否正在运行
        public static async Task<bool> IsOllamaRunningAsync()
        {
            try
            {
                var agent = GetOllamaAgent();
                await agent.GenerateReplyAsync("Test connection");
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 尝试启动Ollama服务
        public static void TryStartOllama()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "ollama",
                    Arguments = "serve",
                    UseShellExecute = true,
                    CreateNoWindow = false
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to start Ollama: {ex.Message}");
            }
        }
    }
} 