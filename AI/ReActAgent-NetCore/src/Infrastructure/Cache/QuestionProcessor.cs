using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ReActAgentNetCore.Core.Interfaces;
using ReActAgentNetCore.Core.Models;

namespace ReActAgentNetCore.Infrastructure.Cache
{
    /// <summary>
    /// 问题预处理服务实现
    /// </summary>
    public class QuestionProcessor : IQuestionProcessor
    {
        private readonly Dictionary<string, List<string>> _intentPatterns;
        private readonly Dictionary<string, List<string>> _keywordPatterns;

        public QuestionProcessor()
        {
            _intentPatterns = InitializeIntentPatterns();
            _keywordPatterns = InitializeKeywordPatterns();
        }

        /// <summary>
        /// 预处理问题
        /// </summary>
        public async Task<ProcessedQuestion> ProcessQuestionAsync(string question)
        {
            var normalizedQuestion = NormalizeQuestion(question);
            var questionHash = GenerateQuestionHash(normalizedQuestion);
            var keywords = await ExtractKeywordsAsync(question);
            var intent = await ExtractIntentAsync(question);

            return new ProcessedQuestion
            {
                OriginalQuestion = question,
                NormalizedQuestion = normalizedQuestion,
                QuestionHash = questionHash,
                Keywords = keywords,
                Intent = intent,
                ExtractedInfo = new Dictionary<string, object>
                {
                    ["word_count"] = question.Split(' ').Length,
                    ["has_special_chars"] = question.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)),
                    ["language"] = DetectLanguage(question)
                }
            };
        }

        /// <summary>
        /// 计算问题相似度
        /// </summary>
        public async Task<double> CalculateSimilarityAsync(string question1, string question2)
        {
            if (string.IsNullOrEmpty(question1) || string.IsNullOrEmpty(question2))
                return 0.0;

            var normalized1 = NormalizeQuestion(question1);
            var normalized2 = NormalizeQuestion(question2);

            // 1. 精确匹配
            if (normalized1.Equals(normalized2, StringComparison.OrdinalIgnoreCase))
                return 1.0;

            // 2. 包含关系
            if (normalized1.Contains(normalized2, StringComparison.OrdinalIgnoreCase) ||
                normalized2.Contains(normalized1, StringComparison.OrdinalIgnoreCase))
                return 0.9;

            // 3. 词汇重叠度
            var words1 = GetWords(normalized1);
            var words2 = GetWords(normalized2);
            var intersection = words1.Intersect(words2, StringComparer.OrdinalIgnoreCase).Count();
            var union = words1.Union(words2, StringComparer.OrdinalIgnoreCase).Count();

            if (union == 0) return 0.0;
            var jaccardSimilarity = (double)intersection / union;

            // 4. 意图匹配
            var intent1 = await ExtractIntentAsync(question1);
            var intent2 = await ExtractIntentAsync(question2);
            var intentBonus = intent1 == intent2 ? 0.2 : 0.0;

            // 5. 关键词匹配
            var keywords1 = await ExtractKeywordsAsync(question1);
            var keywords2 = await ExtractKeywordsAsync(question2);
            var keywordMatches = keywords1.Intersect(keywords2, StringComparer.OrdinalIgnoreCase).Count();
            var keywordBonus = keywordMatches > 0 ? Math.Min(0.3, keywordMatches * 0.1) : 0.0;

            return Math.Min(1.0, jaccardSimilarity + intentBonus + keywordBonus);
        }

        /// <summary>
        /// 提取问题意图
        /// </summary>
        public async Task<string> ExtractIntentAsync(string question)
        {
            var normalizedQuestion = NormalizeQuestion(question).ToLower();

            foreach (var pattern in _intentPatterns)
            {
                if (pattern.Value.Any(p => normalizedQuestion.Contains(p)))
                {
                    return pattern.Key;
                }
            }

            return "general_question";
        }

        /// <summary>
        /// 提取关键词
        /// </summary>
        public async Task<List<string>> ExtractKeywordsAsync(string question)
        {
            var normalizedQuestion = NormalizeQuestion(question);
            var words = GetWords(normalizedQuestion);
            var keywords = new List<string>();

            // 过滤停用词和短词
            var stopWords = new HashSet<string> { "的", "是", "在", "有", "和", "与", "或", "但", "如果", "因为", "所以", "the", "is", "in", "has", "and", "or", "but", "if", "because", "so" };
            
            foreach (var word in words)
            {
                if (word.Length > 1 && !stopWords.Contains(word.ToLower()))
                {
                    keywords.Add(word);
                }
            }

            // 添加技术关键词
            foreach (var pattern in _keywordPatterns)
            {
                if (pattern.Value.Any(k => normalizedQuestion.Contains(k, StringComparison.OrdinalIgnoreCase)))
                {
                    keywords.AddRange(pattern.Value.Where(k => normalizedQuestion.Contains(k, StringComparison.OrdinalIgnoreCase)));
                }
            }

            return keywords.Distinct().ToList();
        }

        #region 私有方法

        private string NormalizeQuestion(string question)
        {
            if (string.IsNullOrEmpty(question))
                return "";

            // 转换为小写
            var normalized = question.ToLower();

            // 移除标点符号
            normalized = Regex.Replace(normalized, @"[^\w\s]", " ");

            // 移除多余空格
            normalized = Regex.Replace(normalized, @"\s+", " ").Trim();

            return normalized;
        }

        private string GenerateQuestionHash(string question)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(question);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private List<string> GetWords(string text)
        {
            return text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                      .Where(w => w.Length > 0)
                      .ToList();
        }

        private string DetectLanguage(string text)
        {
            var chineseChars = text.Count(c => c >= 0x4E00 && c <= 0x9FFF);
            var englishChars = text.Count(c => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'));
            
            if (chineseChars > englishChars)
                return "chinese";
            else if (englishChars > chineseChars)
                return "english";
            else
                return "mixed";
        }

        private Dictionary<string, List<string>> InitializeIntentPatterns()
        {
            return new Dictionary<string, List<string>>
            {
                ["create_project"] = new List<string>
                {
                    "创建", "新建", "建立", "生成", "create", "new", "generate", "build", "make"
                },
                ["file_operation"] = new List<string>
                {
                    "读取", "写入", "创建", "删除", "搜索", "查找", "read", "write", "create", "delete", "search", "find"
                },
                ["code_analysis"] = new List<string>
                {
                    "分析", "检查", "审查", "优化", "重构", "analyze", "review", "optimize", "refactor"
                },
                ["deployment"] = new List<string>
                {
                    "部署", "发布", "运行", "启动", "deploy", "publish", "run", "start"
                },
                ["testing"] = new List<string>
                {
                    "测试", "验证", "检查", "test", "verify", "check", "validate"
                }
            };
        }

        private Dictionary<string, List<string>> InitializeKeywordPatterns()
        {
            return new Dictionary<string, List<string>>
            {
                ["programming_languages"] = new List<string>
                {
                    "c#", "csharp", "dotnet", "net", "javascript", "js", "typescript", "ts", "python", "py", "java", "go", "rust", "cpp", "c++"
                },
                ["frameworks"] = new List<string>
                {
                    "react", "vue", "angular", "next", "nuxt", "express", "fastapi", "django", "flask", "spring", "aspnet", "blazor"
                },
                ["databases"] = new List<string>
                {
                    "sql", "mysql", "postgresql", "mongodb", "redis", "sqlite", "oracle"
                },
                ["cloud_platforms"] = new List<string>
                {
                    "aws", "azure", "gcp", "docker", "kubernetes", "k8s", "terraform"
                },
                ["blockchain"] = new List<string>
                {
                    "solidity", "ethereum", "smart contract", "web3", "defi", "nft"
                }
            };
        }

        #endregion
    }
} 