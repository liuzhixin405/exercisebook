using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReActAgentNetCore.Core.Models;
using ReActAgentNetCore.Core.Interfaces;
using ReActAgentNetCore.Infrastructure.Cache;

namespace ReActAgentNetCore.Application.Agents
{
	/// <summary>
	/// 增强版ReAct智能体 - 集成智能缓存系统
	/// </summary>
	public class EnhancedReActAgent
	{
		private readonly ICacheService _cacheService;
		private readonly IQuestionProcessor _questionProcessor;
		private readonly ISmartRouter _smartRouter;
		private readonly ReActAgent _fallbackAgent;

		public EnhancedReActAgent(
			ReActAgent fallbackAgent,
			ICacheService? cacheService = null,
			IQuestionProcessor? questionProcessor = null,
			ISmartRouter? smartRouter = null)
		{
			_fallbackAgent = fallbackAgent;
			// 初始化缓存系统
			_cacheService = cacheService ?? CreateDefaultCacheService();
			_questionProcessor = questionProcessor ?? new QuestionProcessor();
			_smartRouter = smartRouter ?? new SmartRouter();
		}

		/// <summary>
		/// 运行增强版智能体
		/// </summary>
		public async Task<string> RunAsync(string userInput)
		{
			Console.WriteLine("🔍 正在分析问题...");
			
			// 1. 问题预处理
			var processedQuestion = await _questionProcessor.ProcessQuestionAsync(userInput);
			Console.WriteLine($"📝 问题意图: {processedQuestion.Intent}");
			Console.WriteLine($"🔑 关键词: {string.Join(", ", processedQuestion.Keywords)}");

			// 2. 查询缓存
			Console.WriteLine("🔍 正在查询缓存...");
			var cacheResult = await _cacheService.QueryCacheAsync(userInput);
			
			if (cacheResult.Found)
			{
				Console.WriteLine($"✅ 找到缓存答案 (相似度: {cacheResult.BestSimilarityScore:F2})");
				return await ExecuteCacheStrategy(cacheResult);
			}

			Console.WriteLine("❌ 未找到相关缓存，调用模型处理...");
			// 3. 未命中缓存，调用原有智能体（模型工作流）
			var answer = await _fallbackAgent.RunAsync(userInput);

			// 4. 写回缓存
			try
			{
				await _cacheService.StoreInCacheAsync(userInput, answer, "model");
				Console.WriteLine("💾 已写入缓存");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"⚠️ 写入缓存失败: {ex.Message}");
			}

			return answer;
		}

		private async Task<string> ExecuteCacheStrategy(CacheQueryResult cacheResult)
		{
			if (cacheResult.CacheItem == null)
			{
				return "缓存策略执行失败：缓存项为空";
			}

			// 更新缓存访问信息
			await _cacheService.UpdateAccessInfoAsync(cacheResult.CacheItem.QuestionHash);

			return $"📋 使用缓存答案\n\n" +
				   $"问题: {cacheResult.CacheItem.Question}\n" +
				   $"答案: {cacheResult.CacheItem.Answer}\n" +
				   $"来源: {cacheResult.CacheItem.Source}\n" +
				   $"创建时间: {cacheResult.CacheItem.CreatedAt:yyyy-MM-dd HH:mm:ss}\n" +
				   $"访问次数: {cacheResult.CacheItem.AccessCount}";
		}

		private ICacheService CreateDefaultCacheService()
		{
			var strategy = new CacheStrategy
			{
				MinSimilarityThreshold = 0.8,
				MaxCacheSize = 1000,
				CacheExpiration = TimeSpan.FromDays(30),
				EnableSemanticSearch = true,
				EnableKeywordSearch = true,
				MaxSimilarItems = 5
			};

			var questionProcessor = new QuestionProcessor();
			return new RedisCacheService("localhost:6379", strategy, questionProcessor);
		}
	}
} 