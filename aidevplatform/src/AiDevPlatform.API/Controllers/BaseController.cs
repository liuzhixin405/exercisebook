using Microsoft.AspNetCore.Mvc;
using AiDevPlatform.Shared.DTOs.Common;
using MediatR;

namespace AiDevPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected readonly IMediator Mediator;
    protected readonly ILogger Logger;

    protected BaseController(IMediator mediator, ILogger logger)
    {
        Mediator = mediator;
        Logger = logger;
    }

    protected IActionResult Ok<T>(T data, string? message = null)
    {
        return base.Ok(ApiResponse<T>.SuccessResult(data, message));
    }

    protected IActionResult Ok(string? message = null)
    {
        return base.Ok(ApiResponse.SuccessResult(message));
    }

    protected IActionResult BadRequest(string message, string? errorCode = null, List<string>? errors = null)
    {
        return base.BadRequest(ApiResponse.ErrorResult(message, errorCode, errors));
    }

    protected IActionResult NotFound(string message = "资源未找到")
    {
        return base.NotFound(ApiResponse.ErrorResult(message, "NOT_FOUND"));
    }

    protected IActionResult Unauthorized(string message = "未授权访问")
    {
        return base.Unauthorized(ApiResponse.ErrorResult(message, "UNAUTHORIZED"));
    }

    protected IActionResult Forbidden(string message = "禁止访问")
    {
        return StatusCode(403, ApiResponse.ErrorResult(message, "FORBIDDEN"));
    }

    protected IActionResult InternalServerError(string message = "服务器内部错误")
    {
        return StatusCode(500, ApiResponse.ErrorResult(message, "INTERNAL_SERVER_ERROR"));
    }

    protected IActionResult Created<T>(T data, string? message = null)
    {
        return StatusCode(201, ApiResponse<T>.SuccessResult(data, message));
    }

    protected IActionResult NoContent(string? message = null)
    {
        return StatusCode(204, ApiResponse.SuccessResult(message));
    }

    protected IActionResult Conflict(string message = "资源冲突")
    {
        return StatusCode(409, ApiResponse.ErrorResult(message, "CONFLICT"));
    }

    protected IActionResult TooManyRequests(string message = "请求过于频繁")
    {
        return StatusCode(429, ApiResponse.ErrorResult(message, "TOO_MANY_REQUESTS"));
    }

    protected IActionResult ServiceUnavailable(string message = "服务不可用")
    {
        return StatusCode(503, ApiResponse.ErrorResult(message, "SERVICE_UNAVAILABLE"));
    }

    protected IActionResult GatewayTimeout(string message = "网关超时")
    {
        return StatusCode(504, ApiResponse.ErrorResult(message, "GATEWAY_TIMEOUT"));
    }

    protected IActionResult BadGateway(string message = "网关错误")
    {
        return StatusCode(502, ApiResponse.ErrorResult(message, "BAD_GATEWAY"));
    }

    protected IActionResult NotImplemented(string message = "功能未实现")
    {
        return StatusCode(501, ApiResponse.ErrorResult(message, "NOT_IMPLEMENTED"));
    }

    protected IActionResult UnprocessableEntity(string message = "请求无法处理")
    {
        return StatusCode(422, ApiResponse.ErrorResult(message, "UNPROCESSABLE_ENTITY"));
    }

    protected IActionResult Locked(string message = "资源被锁定")
    {
        return StatusCode(423, ApiResponse.ErrorResult(message, "LOCKED"));
    }

    protected IActionResult FailedDependency(string message = "依赖失败")
    {
        return StatusCode(424, ApiResponse.ErrorResult(message, "FAILED_DEPENDENCY"));
    }

    protected IActionResult TooEarly(string message = "请求过早")
    {
        return StatusCode(425, ApiResponse.ErrorResult(message, "TOO_EARLY"));
    }

    protected IActionResult UpgradeRequired(string message = "需要升级")
    {
        return StatusCode(426, ApiResponse.ErrorResult(message, "UPGRADE_REQUIRED"));
    }

    protected IActionResult PreconditionRequired(string message = "需要前置条件")
    {
        return StatusCode(428, ApiResponse.ErrorResult(message, "PRECONDITION_REQUIRED"));
    }

    protected IActionResult RequestHeaderFieldsTooLarge(string message = "请求头字段过大")
    {
        return StatusCode(431, ApiResponse.ErrorResult(message, "REQUEST_HEADER_FIELDS_TOO_LARGE"));
    }

    protected IActionResult UnavailableForLegalReasons(string message = "因法律原因不可用")
    {
        return StatusCode(451, ApiResponse.ErrorResult(message, "UNAVAILABLE_FOR_LEGAL_REASONS"));
    }

    protected IActionResult InternalServerError(Exception ex, string message = "服务器内部错误")
    {
        Logger.LogError(ex, message);
        return StatusCode(500, ApiResponse.ErrorResult(message, "INTERNAL_SERVER_ERROR"));
    }

    protected IActionResult HandleException(Exception ex, string? customMessage = null)
    {
        Logger.LogError(ex, "An error occurred while processing the request");

        return ex switch
        {
            ArgumentNullException => BadRequest(customMessage ?? ex.Message, "NULL_ARGUMENT"),
            ArgumentException => BadRequest(customMessage ?? ex.Message, "INVALID_ARGUMENT"),
            InvalidOperationException => BadRequest(customMessage ?? ex.Message, "INVALID_OPERATION"),
            UnauthorizedAccessException => Unauthorized(customMessage ?? ex.Message),
            NotImplementedException => NotImplemented(customMessage ?? ex.Message),
            TimeoutException => GatewayTimeout(customMessage ?? ex.Message),
            _ => InternalServerError(ex, customMessage ?? "服务器内部错误")
        };
    }

    protected string GetCurrentUserId()
    {
        return User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value ?? string.Empty;
    }

    protected string GetCurrentUserName()
    {
        return User.FindFirst("name")?.Value ?? User.FindFirst("username")?.Value ?? string.Empty;
    }

    protected string GetCurrentUserEmail()
    {
        return User.FindFirst("email")?.Value ?? string.Empty;
    }

    protected List<string> GetCurrentUserRoles()
    {
        return User.FindAll("role").Select(c => c.Value).ToList();
    }

    protected List<string> GetCurrentUserPermissions()
    {
        return User.FindAll("permission").Select(c => c.Value).ToList();
    }

    protected bool HasRole(string role)
    {
        return GetCurrentUserRoles().Contains(role);
    }

    protected bool HasPermission(string permission)
    {
        return GetCurrentUserPermissions().Contains(permission);
    }

    protected bool HasAnyRole(params string[] roles)
    {
        var userRoles = GetCurrentUserRoles();
        return roles.Any(role => userRoles.Contains(role));
    }

    protected bool HasAnyPermission(params string[] permissions)
    {
        var userPermissions = GetCurrentUserPermissions();
        return permissions.Any(permission => userPermissions.Contains(permission));
    }

    protected bool HasAllRoles(params string[] roles)
    {
        var userRoles = GetCurrentUserRoles();
        return roles.All(role => userRoles.Contains(role));
    }

    protected bool HasAllPermissions(params string[] permissions)
    {
        var userPermissions = GetCurrentUserPermissions();
        return permissions.All(permission => userPermissions.Contains(permission));
    }
}
