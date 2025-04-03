using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pandora.Pipeline.Tools
{
    public class TextDeduplicator
    {
        private readonly ConnectionMultiplexer redis;
        private readonly IDatabase db;

        public TextDeduplicator(string redisConnectionString)
        {
            redis = ConnectionMultiplexer.Connect(redisConnectionString);
            db = redis.GetDatabase();
        }

        // 处理新的文本数据，按日期去重
        public bool Deduplicate(string text, bool isTitle, DateTime timestamp, double similarityThreshold = 0.7)
        {
            string dateKey = $"news:{(isTitle ? "title" : "content")}:dedup:{timestamp:yyyy-MM-dd}";

            // 使用 NormalizeText 清理文本
            var normalizedText = NormalizeText(text);

            bool isDuplicate = false;

            foreach (var existingSignature in db.SetMembers(dateKey))
            {
                // 从Redis中获取已有的文本并进行字符级比较
                string existingText = existingSignature.ToString();
             
                var existingTextList = NormalizeText(existingText);
                double similarity = CalculateCharJaccardSimilarity(normalizedText, existingTextList);

                // 比较相似度
                if (similarity >= similarityThreshold)
                {
                    isDuplicate = true;
                    break;
                }
            }

            if (!isDuplicate)
            {
                db.SetAdd(dateKey, text);  // 将原始文本添加到 Redis
                db.KeyExpire(dateKey, TimeSpan.FromDays(1));
            }

            return isDuplicate;
        }

        // NormalizeText: 去除标点符号，转为小写，按字符拆分
        public static List<string> NormalizeText(string text)
        {
            text = text.ToLower();

            // 过滤掉以"xx: "或"xx："开头的字符，冒号可以是英文或中文，冒号和字符之间可有空格
            text = Regex.Replace(text, @"^[^\s:：]+\s*[:：]\s*", "");

            // 去除标点符号
            text = Regex.Replace(text, @"[^\w\s]", "");

            // 按字符拆分并转换为列表
            return text.ToCharArray().Select(c => c.ToString()).ToList();
        }

        // 计算字符集合的Jaccard相似度
        public double CalculateCharJaccardSimilarity(List<string> tokens1, List<string> tokens2)
        {
            var set1 = new HashSet<string>(tokens1);
            var set2 = new HashSet<string>(tokens2);

            // 计算交集和并集
            var intersection = set1.Intersect(set2).Count();
            var union = set1.Union(set2).Count();

            // 计算 Jaccard 相似度
            return (double)intersection / union;
        }
    }
}
