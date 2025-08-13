using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReActAgentNetCore.Core.Interfaces;
using ReActAgentNetCore.Core.Models;

namespace ReActAgentNetCore.Infrastructure.Cache
{
	/// <summary>
	/// 最小实现的智能路由器，确保编译通过。
	/// </summary>
	public class SmartRouter : ISmartRouter
	{
		public Task<RoutingDecision> DecideRoutingAsync(string question, CacheQueryResult? cacheResult = null)
		{
			// 简化策略：如果有高相似度缓存则使用缓存，否则调用模型
			if (cacheResult != null && cacheResult.Found && cacheResult.BestSimilarityScore >= 0.9)
			{
				return Task.FromResult(new RoutingDecision
				{
					Strategy = "use_cache",
					CacheItem = cacheResult.CacheItem,
					Confidence = cacheResult.BestSimilarityScore,
					Reason = "命中高相似度缓存",
					Parameters = new Dictionary<string, object>
					{
						["query_strategy"] = cacheResult.QueryStrategy,
						["similarity_score"] = cacheResult.BestSimilarityScore
					}
				});
			}

			return Task.FromResult(new RoutingDecision
			{
				Strategy = "call_model",
				Confidence = 0.6,
				Reason = "默认使用模型处理",
				Parameters = new Dictionary<string, object>()
			});
		}

		public Task<string> ExecuteRoutingAsync(RoutingDecision decision)
		{
			// 最小实现：直接返回策略说明
			var result = $"策略: {decision.Strategy}\n原因: {decision.Reason}\n置信度: {decision.Confidence:F2}";
			return Task.FromResult(result);
		}
	}
} 