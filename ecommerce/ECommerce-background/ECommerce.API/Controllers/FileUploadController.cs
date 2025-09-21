using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FileUploadController : BaseController
    {
        private readonly ILogger<FileUploadController> _logger;
        private readonly IWebHostEnvironment _environment;
        private const string UploadFolder = "uploads";
        private const string ImageFolder = "images";
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public FileUploadController(ILogger<FileUploadController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        /// <summary>
        /// 检查当前用户是否为管理员
        /// </summary>
        private bool IsAdmin()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            return roleClaim?.Value == "Admin";
        }

        /// <summary>
        /// 上传产品图片
        /// </summary>
        [HttpPost("product-image")]
        public async Task<ActionResult<object>> UploadProductImage(IFormFile file)
        {
            // 检查管理员权限
            if (!IsAdmin())
            {
                _logger.LogWarning("Non-admin user {UserId} attempted to upload image", CurrentUserId);
                return Forbid("Access denied. Admin privileges required.");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            // 验证文件大小
            if (file.Length > MaxFileSize)
            {
                return BadRequest($"File size exceeds the maximum limit of {MaxFileSize / (1024 * 1024)}MB");
            }

            // 验证文件扩展名
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(fileExtension))
            {
                return BadRequest($"File type not allowed. Allowed types: {string.Join(", ", AllowedExtensions)}");
            }

            try
            {
                // 确保WebRootPath存在
                if (string.IsNullOrEmpty(_environment.WebRootPath))
                {
                    _logger.LogError("WebRootPath is null or empty");
                    return StatusCode(500, "Server configuration error");
                }

                // 创建上传目录
                var uploadPath = Path.Combine(_environment.WebRootPath, UploadFolder, ImageFolder);
                _logger.LogInformation("Upload path: {UploadPath}", uploadPath);
                
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                    _logger.LogInformation("Created upload directory: {UploadPath}", uploadPath);
                }

                // 生成唯一文件名
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadPath, fileName);

                // 保存文件
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // 生成访问URL
                var fileUrl = $"/{UploadFolder}/{ImageFolder}/{fileName}";

                _logger.LogInformation("Admin {AdminId} uploaded image: {FileName} to {FilePath}", CurrentUserId, fileName, filePath);

                return Ok(new
                {
                    success = true,
                    fileName = fileName,
                    fileUrl = fileUrl,
                    fileSize = file.Length,
                    message = "Image uploaded successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image: {Message}", ex.Message);
                return StatusCode(500, $"Error uploading image: {ex.Message}");
            }
        }

        /// <summary>
        /// 删除产品图片
        /// </summary>
        [HttpDelete("product-image/{fileName}")]
        public async Task<ActionResult> DeleteProductImage(string fileName)
        {
            // 检查管理员权限
            if (!IsAdmin())
            {
                _logger.LogWarning("Non-admin user {UserId} attempted to delete image {FileName}", CurrentUserId, fileName);
                return Forbid("Access denied. Admin privileges required.");
            }

            try
            {
                var filePath = Path.Combine(_environment.WebRootPath, UploadFolder, ImageFolder, fileName);

                if (System.IO.File.Exists (filePath))
                {
                    System.IO.File.Delete(filePath);
                    _logger.LogInformation("Admin {AdminId} deleted image: {FileName}", CurrentUserId, fileName);
                    return Ok(new { success = true, message = "Image deleted successfully" });
                }
                else
                {
                    return NotFound("Image not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image {FileName}", fileName);
                return StatusCode(500, "Error deleting image");
            }
        }

        /// <summary>
        /// 获取所有已上传的图片列表
        /// </summary>
        [HttpGet("product-images")]
        public ActionResult<object> GetProductImages()
        {
            // 检查管理员权限
            if (!IsAdmin())
            {
                _logger.LogWarning("Non-admin user {UserId} attempted to list images", CurrentUserId);
                return Forbid("Access denied. Admin privileges required.");
            }

            try
            {
                // 确保WebRootPath存在
                if (string.IsNullOrEmpty(_environment.WebRootPath))
                {
                    _logger.LogError("WebRootPath is null or empty");
                    return StatusCode(500, "Server configuration error");
                }

                var uploadPath = Path.Combine(_environment.WebRootPath, UploadFolder, ImageFolder);
                _logger.LogInformation("Getting images from path: {UploadPath}", uploadPath);
                
                if (!Directory.Exists(uploadPath))
                {
                    _logger.LogInformation("Upload directory does not exist: {UploadPath}", uploadPath);
                    return Ok(new { images = new List<object>() });
                }

                var imageFiles = Directory.GetFiles(uploadPath)
                    .Where(file => AllowedExtensions.Contains(Path.GetExtension(file).ToLowerInvariant()))
                    .Select(file =>
                    {
                        var fileName = Path.GetFileName(file);
                        var fileInfo = new FileInfo(file);
                        return new
                        {
                            fileName = fileName,
                            fileUrl = $"/{UploadFolder}/{ImageFolder}/{fileName}",
                            fileSize = fileInfo.Length,
                            uploadDate = fileInfo.CreationTime
                        };
                    })
                    .OrderByDescending(img => img.uploadDate)
                    .ToList();

                _logger.LogInformation("Found {Count} images", imageFiles.Count);
                return Ok(new { images = imageFiles });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting image list: {Message}", ex.Message);
                return StatusCode(500, $"Error getting image list: {ex.Message}");
            }
        }
    }
}
