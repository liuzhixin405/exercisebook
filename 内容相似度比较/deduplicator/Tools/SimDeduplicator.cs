using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 内容相似度比较.Tools
{
    using SimMetricsCore.API;
    using StackExchange.Redis;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.Json;
    using System.Text.RegularExpressions;

    namespace Pandora.Pipeline.Tools
    {
        public class SimDeduplicator
        {
            private readonly ConnectionMultiplexer redis;
            private readonly IDatabase db;
            private readonly int simHashBits = 64;

            public SimDeduplicator(string redisConnectionString)
            {
                redis = ConnectionMultiplexer.Connect(redisConnectionString);
                db = redis.GetDatabase();
            }

            public bool Deduplicate(string text, bool isTitle, DateTime timestamp, int hammingThreshold = 3)
            {
                string dateKey = $"news:{(isTitle ? "title" : "content")}:dedup:{timestamp:yyyy-MM-dd}";

                string normalizedText = NormalizeText(text);
                ulong currentHash = ComputeSimHash(normalizedText);

                foreach (var item in db.SetMembers(dateKey))
                {
                    var json = item.ToString();
                    var existing = JsonSerializer.Deserialize<DedupItem>(json);
                    if (existing == null) continue;

                    int distance = HammingDistance(currentHash, existing.Hash);
                    if (distance <= hammingThreshold)
                    {
                        return true; // 是重复的
                    }
                }

                // 不重复，保存
                var newItem = new DedupItem
                {
                    Text = text,
                    Hash = currentHash
                };

                db.SetAdd(dateKey, JsonSerializer.Serialize(newItem));
                db.KeyExpire(dateKey, TimeSpan.FromDays(1));

                return false;
            }

            public static string NormalizeText(string text)
            {
                text = text.ToLower();
                text = Regex.Replace(text, @"^[^\s:：]+\s*[:：]\s*", "");
                text = Regex.Replace(text, @"[^\w\s]", "");
                return text.Trim();
            }

            private ulong ComputeSimHash(string text)
            {
                int[] v = new int[simHashBits];
                var tokens = text.ToCharArray().Select(c => c.ToString()).ToList();

                var freq = tokens.GroupBy(t => t).ToDictionary(g => g.Key, g => g.Count());

                foreach (var pair in freq)
                {
                    ulong tokenHash = Hash64(pair.Key);
                    int weight = pair.Value;

                    for (int i = 0; i < simHashBits; i++)
                    {
                        int bit = ((tokenHash >> i) & 1) == 1 ? 1 : -1;
                        v[i] += bit * weight;
                    }
                }

                ulong fingerprint = 0;
                for (int i = 0; i < simHashBits; i++)
                {
                    if (v[i] > 0)
                        fingerprint |= (1UL << i);
                }
                return fingerprint;
            }

            private ulong Hash64(string word)
            {
                using (SHA256 sha = SHA256.Create())
                {
                    byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(word));
                    return BitConverter.ToUInt64(hash, 0); // 取前64位
                }
            }

            private int HammingDistance(ulong x, ulong y)
            {
                ulong xor = x ^ y;
                int dist = 0;
                while (xor != 0)
                {
                    xor &= (xor - 1);
                    dist++;
                }
                return dist;
            }

            private class DedupItem
            {
                public string Text { get; set; }
                public ulong Hash { get; set; }
            }
        }
    }

}
