using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ReActAgentNetCore.Infrastructure.Tools;
using ReActAgentNetCore.Application.Agents;
using ReActAgentNetCore.Infrastructure.Cache;
using ReActAgentNetCore.Core.Models;

namespace ReActAgentNetCore.Application.CLI
{
	class Program
	{
		static async Task Main(string[] args)
		{
			try
			{
				// 构建配置
				var builder = new ConfigurationBuilder()
					.SetBasePath(AppContext.BaseDirectory)
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

				IConfiguration configuration = builder.Build();

				Console.WriteLine("🤖 ReAct智能体 - .NET Core版本");
				Console.WriteLine("=====================================");
				Console.WriteLine();

				// 获取项目目录
				string projectDirectory;
				if (args.Length > 0)
				{
					projectDirectory = args[0];
				}
				else
				{
					var defaultProjectDirectory = configuration["Agent:DefaultProjectDirectory"];
					if (!string.IsNullOrWhiteSpace(defaultProjectDirectory) && Directory.Exists(defaultProjectDirectory))
					{
						projectDirectory = defaultProjectDirectory;
					}
					else
					{
						Console.Write("请输入项目目录路径（或按回车使用默认的'proj'目录）：");
						var input = Console.ReadLine();
						if (string.IsNullOrWhiteSpace(input))
						{
							// 创建默认的proj目录
							projectDirectory = Path.Combine(Directory.GetCurrentDirectory(), "proj");
							if (!Directory.Exists(projectDirectory))
							{
								Directory.CreateDirectory(projectDirectory);
								Console.WriteLine($"✅ 已创建默认项目目录：{projectDirectory}");
							}
						}
						else
						{
							projectDirectory = input;
						}
					}
				}

				// 验证目录是否存在，如果不存在则创建（针对proj目录）
				if (!Directory.Exists(projectDirectory))
				{
					// 如果是默认的proj目录，则创建它
					if (Path.GetFileName(projectDirectory) == "proj")
					{
						Directory.CreateDirectory(projectDirectory);
						Console.WriteLine($"✅ 已创建项目目录：{projectDirectory}");
					}
					else
					{
						Console.WriteLine($"❌ 目录不存在：{projectDirectory}");
						return;
					}
				}

				projectDirectory = Path.GetFullPath(projectDirectory);
				Console.WriteLine($"📁 工作目录：{projectDirectory}");
				Console.WriteLine();

				// 从配置获取模型和Ollama设置
				var model = configuration["Ollama:Model"] ?? "qwen2.5-coder:7b";
				var ollamaBaseUrl = configuration["Ollama:BaseUrl"] ?? "http://localhost:11434";

				// 设置工具的项目目录
				Tools.SetProjectDirectory(projectDirectory);
				
				// 创建工具字典
				var tools = new Dictionary<string, Delegate>
				{
					{ "read_file", Tools.ReadFileAsync },
					{ "write_to_file", Tools.WriteToFileAsync },
					{ "run_terminal_command", Tools.RunTerminalCommandAsync },
					{ "list_directory", Tools.ListDirectoryAsync },
					{ "search_files", Tools.SearchFilesAsync },
					{ "create_directory", Tools.CreateDirectoryAsync },
					{ "create_dotnet_project", Tools.CreateDotNetProjectAsync },
					{ "create_node_project", Tools.CreateNodeProjectAsync },
					{ "create_python_project", Tools.CreatePythonProjectAsync },
					{ "create_blockchain_project", Tools.CreateBlockchainProjectAsync },
					{ "create_mobile_project", Tools.CreateMobileProjectAsync },
					{ "create_project_via_mcp", Tools.CreateProjectViaMCPAsync }
				};

				// 原有智能体实例（作为回退）
				var baseAgent = new ReActAgent(
					tools: tools,
					model: model,
					projectDirectory: projectDirectory,
					ollamaBaseUrl: ollamaBaseUrl
				);

				// 初始化缓存服务
				var cacheEnabled = bool.TryParse(configuration["Cache:Enabled"], out var ce) ? ce : true;
				var strategy = new CacheStrategy
				{
					MinSimilarityThreshold = double.TryParse(configuration["Cache:MinSimilarityThreshold"], out var mst) ? mst : 0.8,
					MaxCacheSize = int.TryParse(configuration["Cache:MaxCacheSize"], out var mcs) ? mcs : 1000,
					CacheExpiration = TimeSpan.FromDays(int.TryParse(configuration["Cache:CacheExpirationDays"], out var ced) ? ced : 30),
					EnableSemanticSearch = bool.TryParse(configuration["Cache:EnableSemanticSearch"], out var ess) ? ess : true,
					EnableKeywordSearch = bool.TryParse(configuration["Cache:EnableKeywordSearch"], out var eks) ? eks : true,
					MaxSimilarItems = int.TryParse(configuration["Cache:MaxSimilarItems"], out var msi) ? msi : 5
				};
				var questionProcessor = new QuestionProcessor();
				var cacheService = new RedisCacheService(configuration["Cache:RedisConnectionString"] ?? "localhost:6379", strategy, questionProcessor);

				// 增强版智能体（带缓存与回退）
				var agent = new EnhancedReActAgent(baseAgent, cacheEnabled ? cacheService : null, questionProcessor);

				Console.WriteLine("✅ 智能体初始化完成！");
				Console.WriteLine("💡 使用说明：");
				Console.WriteLine($"   - 确保Ollama服务正在运行 ({ollamaBaseUrl})");
				Console.WriteLine($"   - 确保已下载 {model} 模型");
				Console.WriteLine("   - 输入任务描述，智能体会自动执行");
				Console.WriteLine();

				// 主循环
				while (true)
				{
					Console.Write("🎯 请输入任务（输入'exit'退出）：");
					var task = Console.ReadLine();

					if (string.IsNullOrWhiteSpace(task))
						continue;

					if (task.ToLower() == "exit")
						break;

					try
					{
						Console.WriteLine("\n🚀 开始执行任务...");
						var finalAnswer = await agent.RunAsync(task);
						Console.WriteLine($"\n✅ 任务完成！");
						Console.WriteLine($"📝 最终答案：{finalAnswer}");
					}
					catch (Exception ex)
					{
						Console.WriteLine($"\n❌ 任务执行失败：{ex.Message}");
						if (ex.InnerException != null)
						{
							Console.WriteLine($"   详细错误：{ex.InnerException.Message}");
						}
					}

					Console.WriteLine("\n" + new string('=', 50) + "\n");
				}

				Console.WriteLine("👋 再见！");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ 程序启动失败：{ex.Message}");
				Console.WriteLine("请检查：");
				Console.WriteLine("1. Ollama服务是否正在运行");
				Console.WriteLine("2. 网络连接是否正常");
				Console.WriteLine("3. 项目目录路径是否正确");
				Console.WriteLine("4. appsettings.json配置是否正确");
			}
		}
	}
} 