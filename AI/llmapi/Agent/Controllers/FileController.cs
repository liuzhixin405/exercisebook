using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AiAgent.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AiAgent.Controllers
{
    [ApiController]
    [Route("api/file")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<FileController> _logger;

        public FileController(IFileService fileService, ILogger<FileController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromForm] string? relativePath = null)
        {
            try
            {
                var fileName = await _fileService.SaveFileAsync(file, relativePath);
                return Ok(new { fileName = fileName, message = "文件上传成功" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "文件上传失败");
                return StatusCode(500, new { error = "文件上传失败", message = ex.Message });
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListFiles([FromQuery] string? path = null)
        {
            try
            {
                var files = await _fileService.ListCachedFilesAsync(path);
                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "列出文件失败");
                return StatusCode(500, new { error = "列出文件失败", message = ex.Message });
            }
        }

        [HttpDelete("delete/{fileName}")]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            try
            {
                var success = await _fileService.DeleteFileAsync(fileName);
                if (success)
                {
                    return Ok(new { message = $"文件 '{fileName}' 已删除" });
                }
                return NotFound(new { message = $"文件 '{fileName}' 不存在" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除文件 '{fileName}' 失败");
                return StatusCode(500, new { error = "删除文件失败", message = ex.Message });
            }
        }

        [HttpGet("content/{fileName}")]
        public async Task<IActionResult> GetFileContent(string fileName)
        {
            try
            {
                var content = await _fileService.ReadFileContentAsync(fileName);
                return Ok(new { fileName = fileName, content = content });
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"读取文件 '{fileName}' 内容失败");
                return StatusCode(500, new { error = "读取文件内容失败", message = ex.Message });
            }
        }

        [HttpDelete("delete-all")]
        public async Task<IActionResult> DeleteAllFiles([FromQuery] string? path = null)
        {
            try
            {
                var success = await _fileService.DeleteAllFilesAsync(path);
                if (success)
                {
                    return Ok(new { message = "所有缓存文件已删除" });
                }
                return StatusCode(500, new { message = "删除所有文件失败" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除所有文件失败");
                return StatusCode(500, new { error = "删除所有文件失败", message = ex.Message });
            }
        }

        [HttpPost("create-folder")]
        public async Task<IActionResult> CreateFolder([FromBody] CreateFolderRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FolderName))
            {
                return BadRequest(new { message = "文件夹名称不能为空。" });
            }

            try
            {
                var success = await _fileService.CreateFolderAsync(request.FolderName);
                if (success)
                {
                    return Ok(new { message = $"文件夹 '{request.FolderName}' 已成功创建" });
                }
                return Conflict(new { message = $"文件夹 '{request.FolderName}' 已存在或创建失败" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"创建文件夹 '{request.FolderName}' 失败");
                return StatusCode(500, new { error = "创建文件夹失败", message = ex.Message });
            }
        }
    }

    public class CreateFolderRequest
    {
        public string FolderName { get; set; } = string.Empty;
    }
} 