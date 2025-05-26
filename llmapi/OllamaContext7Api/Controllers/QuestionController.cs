using Microsoft.AspNetCore.Mvc;
using OllamaContext7Api.Services;

namespace OllamaContext7Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IAIService _aiService;
        private readonly McpServerService _mcpService;
        private readonly ILogger<QuestionController> _logger;

        // 配置开关 - 可以通过配置文件控制是否使用Context7
        private readonly bool _useContext7 = true; // 设为false可以完全禁用Context7

        public QuestionController(
            IAIService aiService,
            McpServerService mcpService,
            ILogger<QuestionController> logger)
        {
            _aiService = aiService;
            _mcpService = mcpService;
            _logger = logger;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskQuestion([FromBody] QuestionRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Question))
                {
                    return BadRequest("问题不能为空");
                }

                _logger.LogInformation($"收到问题: {request.Question}");

                string docs = "";
                string libraryUsed = "";
                bool hasDocumentation = false;

                // 如果启用Context7，尝试获取相关文档
                if (_useContext7)
                {
                    try
                    {
                        // 1. 启动MCP服务器
                        await _mcpService.StartMcpServerAsync();

                        // 2. 智能确定查询库和关键词
                        var (library, keywords) = AnalyzeQuestionForLibrary(request.Question);
                        libraryUsed = library;

                        _logger.LogInformation($"确定查询库: {library}, 关键词: {string.Join(", ", keywords)}");

                        // 3. 通过MCP获取相关文档，使用优化的关键词而不是原始问题
                        var optimizedQuery = string.Join(" ", keywords.Take(3)); // 只使用前3个最重要的关键词
                        docs = await _mcpService.GetLibraryDocsAsync(library, optimizedQuery, optimizedQuery);

                        // 4. 验证文档质量
                        if (IsDocumentUseful(docs, keywords))
                        {
                            hasDocumentation = true;
                            _logger.LogInformation($"获取到有用文档，长度: {docs.Length}");
                        }
                        else
                        {
                            docs = "";
                            hasDocumentation = false;
                            _logger.LogInformation("获取的文档质量不高，将使用通用知识回答");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Context7获取文档失败: {ex.Message}，将使用通用知识回答");
                        docs = "";
                        hasDocumentation = false;
                    }
                }

                // 5. 使用AI模型处理问题
                var answer = await _aiService.ProcessWithContextAsync(request.Question, docs);

                return Ok(new QuestionResponse
                {
                    Question = request.Question,
                    Answer = answer,
                    LibraryUsed = libraryUsed,
                    HasDocumentation = hasDocumentation,
                    UseContext7 = _useContext7
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"处理问题时出错: {request?.Question}");
                return StatusCode(500, new
                {
                    error = "处理问题时出错",
                    message = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("test-mcp")]
        public async Task<IActionResult> TestMcp([FromQuery] string library = "dotnet", [FromQuery] string query = "getting started")
        {
            try
            {
                if (!_useContext7)
                {
                    return Ok(new { message = "Context7已禁用", enabled = false });
                }

                await _mcpService.StartMcpServerAsync();
                var docs = await _mcpService.GetLibraryDocsAsync(library, query, query);

                var keywords = ExtractTechnicalKeywords(query);
                var isUseful = IsDocumentUseful(docs, keywords);

                return Ok(new
                {
                    library = library,
                    query = query,
                    success = true,
                    documentLength = docs?.Length ?? 0,
                    isUseful = isUseful,
                    keywords = keywords,
                    preview = docs?.Length > 200 ? docs.Substring(0, 200) + "..." : docs
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    library = library,
                    query = query,
                    success = false,
                    error = ex.Message
                });
            }
        }

        [HttpGet("supported-libraries")]
        public IActionResult GetSupportedLibraries()
        {
            var libraries = new[]
            {
                new {
                    name = "dotnet",
                    keywords = new[] { "dotnet", ".net", "netcore", "aspnet", "c#", "csharp", "blazor", "webapi", "mvc", "ef", "entity framework" },
                    description = ".NET生态系统文档"
                },
                new {
                    name = "react",
                    keywords = new[] { "react", "jsx", "hooks", "component", "state", "props" },
                    description = "React框架文档"
                },
                new {
                    name = "vue",
                    keywords = new[] { "vue", "vuejs", "composition", "options" },
                    description = "Vue.js框架文档"
                },
                new {
                    name = "express",
                    keywords = new[] { "express", "expressjs", "node", "middleware", "routing" },
                    description = "Express.js框架文档"
                },
                new {
                    name = "mongodb",
                    keywords = new[] { "mongodb", "mongo", "database", "nosql", "collection" },
                    description = "MongoDB数据库文档"
                }
            };

            return Ok(new { libraries, context7Enabled = _useContext7 });
        }

        /// <summary>
        /// 智能分析问题，确定最合适的库和关键词
        /// </summary>
        private (string library, List<string> keywords) AnalyzeQuestionForLibrary(string question)
        {
            var questionLower = question.ToLower();
            var keywords = ExtractTechnicalKeywords(question);

            // 按优先级检查库匹配
            var libraryMappings = new[]
            {
                (new[] { "dotnet", ".net", "netcore", "aspnet", "c#", "csharp", "blazor", "webapi", "mvc", "ef", "entity framework", "依赖注入", "中间件" }, "dotnet"),
                (new[] { "react", "jsx", "hooks", "component", "useState", "useEffect" }, "react"),
                (new[] { "vue", "vuejs", "composition api", "options api" }, "vue"),
                (new[] { "express", "expressjs", "node", "nodejs", "middleware", "routing" }, "express"),
                (new[] { "mongodb", "mongo", "database", "nosql", "collection", "document" }, "mongodb")
            };

            foreach (var (libraryKeywords, libraryName) in libraryMappings)
            {
                if (libraryKeywords.Any(kw => questionLower.Contains(kw)))
                {
                    // 过滤出与该库相关的关键词
                    var relevantKeywords = keywords.Where(k =>
                        libraryKeywords.Any(lk => lk.Contains(k.ToLower()) || k.ToLower().Contains(lk))).ToList();

                    // 如果没有相关关键词，使用通用关键词
                    if (!relevantKeywords.Any())
                    {
                        relevantKeywords = keywords.Take(3).ToList();
                    }

                    return (libraryName, relevantKeywords);
                }
            }

            // 默认使用dotnet，返回所有关键词
            return ("dotnet", keywords);
        }

        /// <summary>
        /// 提取技术关键词，过滤掉无用词汇
        /// </summary>
        private List<string> ExtractTechnicalKeywords(string text)
        {
            var keywords = new List<string>();

            // 技术术语优先级更高
            var highPriorityTerms = new[]
            {
                "api", "controller", "service", "model", "dto", "entity", "repository",
                "async", "await", "task", "http", "json", "xml", "database", "sql",
                "authentication", "authorization", "middleware", "dependency injection",
                "configuration", "logging", "testing", "exception", "validation",
                "cors", "jwt", "oauth", "swagger", "openapi", "ef core", "blazor",
                "webapi", "mvc", "razor", "signalr", "grpc", "microservice",
                "react", "vue", "angular", "component", "hooks", "state", "props",
                "mongodb", "express", "nodejs", "routing"
            };

            var textLower = text.ToLower();

            // 添加高优先级术语
            keywords.AddRange(highPriorityTerms.Where(term => textLower.Contains(term)));

            // 提取其他有意义的词汇（3个字母以上，非停用词）
            var words = System.Text.RegularExpressions.Regex.Matches(text, @"\b\w{3,}\b")
                .Cast<System.Text.RegularExpressions.Match>()
                .Select(m => m.Value.ToLower())
                .Where(w => !IsStopWord(w) && !keywords.Contains(w))
                .Take(5)
                .ToList();

            keywords.AddRange(words);

            return keywords.Distinct().Take(8).ToList(); // 限制关键词数量
        }

        /// <summary>
        /// 判断文档是否有用
        /// </summary>
        private bool IsDocumentUseful(string docs, List<string> keywords)
        {
            if (string.IsNullOrWhiteSpace(docs) || docs.Length < 100)
                return false;

            // 检查是否包含错误信息
            var errorIndicators = new[] { "error", "failed", "无法", "失败", "错误", "not found" };
            if (errorIndicators.Any(indicator => docs.ToLower().Contains(indicator)))
                return false;

            // 检查是否包含相关关键词
            var docsLower = docs.ToLower();
            var keywordMatches = keywords.Count(keyword => docsLower.Contains(keyword.ToLower()));

            // 至少包含25%的关键词才认为有用
            var relevanceThreshold = Math.Max(1, keywords.Count * 0.25);

            // 检查是否包含代码或技术内容
            var hasTechnicalContent = docs.Contains("```") ||
                                    docs.Contains("class ") ||
                                    docs.Contains("public ") ||
                                    docs.Contains("function ") ||
                                    docs.Contains("const ") ||
                                    docs.Contains("var ") ||
                                    docs.Contains("let ");

            return keywordMatches >= relevanceThreshold || hasTechnicalContent;
        }

        private bool IsStopWord(string word)
        {
            var stopWords = new HashSet<string>
            {
                "the", "a", "an", "and", "or", "but", "in", "on", "at", "to", "for",
                "of", "with", "by", "from", "as", "is", "are", "was", "were", "be",
                "have", "has", "had", "do", "does", "did", "will", "would", "could",
                "should", "may", "might", "can", "how", "what", "when", "where", "why",
                "this", "that", "these", "those", "i", "you", "he", "she", "it", "we", "they",
                "请", "帮", "我", "想", "需要", "如何", "怎么", "什么", "为什么", "哪里", "哪个"
            };

            return stopWords.Contains(word.ToLower());
        }
    }

    public class QuestionRequest
    {
        public string Question { get; set; } = "";
    }

    public class QuestionResponse
    {
        public string Question { get; set; } = "";
        public string Answer { get; set; } = "";
        public string LibraryUsed { get; set; } = "";
        public bool HasDocumentation { get; set; }
        public bool UseContext7 { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}