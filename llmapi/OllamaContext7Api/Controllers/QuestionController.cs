using Microsoft.AspNetCore.Mvc;
using OllamaContext7Api.Services;

namespace OllamaContext7Api.Controllers
{
    [ApiController]
    [Route("question")]
    public class QuestionController : ControllerBase
    {
        private readonly IAIService _aiService;

        public QuestionController(IAIService aiService, McpServerService mcp)
        {
            _aiService = aiService;
            _mcp = mcp;
            _mcp.StartMcpServer(); // 确保只启动一次
        }
        private readonly McpServerService _mcp;

        
        [HttpPost("Que")]
        public async Task<IActionResult> Que([FromBody] string question)
        {
            try
            {
                // 1. 通过MCP获取相关文档
                var docs = await _mcp.GetLibraryDocsAsync("dotnet", question);
                
                // 2. 使用AI模型处理问题和文档
                var answer = await _aiService.ProcessWithContextAsync(question, docs);
                
                return Ok(answer);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("MCP"))
            {
                // MCP服务相关错误
                return StatusCode(503, $"文档服务暂时不可用: {ex.Message}");
            }
            catch (Exception ex)
            {
                // 其他错误
                return StatusCode(500, $"处理问题时出错: {ex.Message}");
            }
        }
    }
}
