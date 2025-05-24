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

                // 1. 启动MCP服务器
                await _mcpService.StartMcpServerAsync();

                // 2. 确定要查询的库
                var libraryName = DetermineLibraryFromQuestion(request.Question);
                _logger.LogInformation($"确定查询库: {libraryName}");

                // 3. 通过MCP获取相关文档
                string docs = "";
                try
                {
                    docs = await _mcpService.GetLibraryDocsAsync(libraryName, request.Question);
                    _logger.LogInformation($"获取到文档，长度: {docs?.Length ?? 0}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"获取文档失败: {ex.Message}");
                    docs = "无法获取相关文档，将基于通用知识回答";
                }

                // 4. 使用AI模型处理问题和文档
                var answer = await _aiService.ProcessWithContextAsync(request.Question, docs);

                return Ok(new QuestionResponse
                {
                    Question = request.Question,
                    Answer = answer,
                    LibraryUsed = libraryName,
                    HasDocumentation = !string.IsNullOrEmpty(docs) && !docs.Contains("无法获取")
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
        public async Task<IActionResult> TestMcp([FromQuery] string library = "dotnet")
        {
            try
            {
                await _mcpService.StartMcpServerAsync();
                var docs = await _mcpService.GetLibraryDocsAsync(library, "getting started");

                return Ok(new
                {
                    library = library,
                    success = true,
                    documentLength = docs?.Length ?? 0,
                    preview = docs?.Length > 200 ? docs.Substring(0, 200) + "..." : docs
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    library = library,
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
                new { name = "dotnet", keywords = new[] { "dotnet", ".net", "netcore", "aspnet", "c#", "csharp" } },
                new { name = "react", keywords = new[] { "react", "jsx", "hooks" } },
                new { name = "vue", keywords = new[] { "vue", "vuejs" } },
                new { name = "express", keywords = new[] { "express", "expressjs", "node" } },
                new { name = "mongodb", keywords = new[] { "mongodb", "mongo", "database" } }
            };

            return Ok(libraries);
        }

        private string DetermineLibraryFromQuestion(string question)
        {
            var questionLower = question.ToLower();

            // .NET 相关关键词
            if (questionLower.Contains("dotnet") ||
                questionLower.Contains(".net") ||
                questionLower.Contains("netcore") ||
                questionLower.Contains("aspnet") ||
                questionLower.Contains("c#") ||
                questionLower.Contains("csharp"))
            {
                return "dotnet";
            }

            // React 相关
            if (questionLower.Contains("react") ||
                questionLower.Contains("jsx") ||
                questionLower.Contains("hooks"))
            {
                return "react";
            }

            // Vue 相关
            if (questionLower.Contains("vue"))
            {
                return "vue";
            }

            // Express 相关
            if (questionLower.Contains("express") ||
                questionLower.Contains("node"))
            {
                return "express";
            }

            // MongoDB 相关
            if (questionLower.Contains("mongodb") ||
                questionLower.Contains("mongo"))
            {
                return "mongodb";
            }

            // 默认返回 dotnet
            return "dotnet";
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
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}