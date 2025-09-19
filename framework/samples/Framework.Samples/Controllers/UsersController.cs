using Framework.Core.Abstractions.Commands;
using Framework.Core.Abstractions.Events;
using Framework.Samples.Commands;
using Framework.Samples.Events;
using Framework.Samples.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Framework.Samples.Controllers;

/// <summary>
/// 用户控制器
/// 演示框架的各种设计模式
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICommandBus _commandBus;
    private readonly IEventBus _eventBus;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="userService">用户服务</param>
    /// <param name="commandBus">命令总线</param>
    /// <param name="eventBus">事件总线</param>
    public UsersController(IUserService userService, ICommandBus commandBus, IEventBus eventBus)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _commandBus = commandBus ?? throw new ArgumentNullException(nameof(commandBus));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    /// <param name="request">创建用户请求</param>
    /// <returns>新创建的用户ID</returns>
    /// <response code="200">用户创建成功</response>
    /// <response code="400">请求参数无效</response>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CreateUser([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand
        {
            UserName = request.UserName,
            Email = request.Email,
            Password = request.Password
        };

        await _commandBus.SendAsync(command);

        // 通过服务获取创建的用户ID（这里简化处理）
        var usersResult = await GetAllUsers();
        var users = usersResult.Value ?? new List<User>();
        var user = users.FirstOrDefault(u => u.Name == request.UserName && u.Email == request.Email);
        
        return Ok(user?.UserId ?? Guid.NewGuid());
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="request">更新用户请求</param>
    /// <returns>更新结果</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<bool>> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        var command = new UpdateUserCommand
        {
            UserId = id,
            UserName = request.UserName,
            Email = request.Email
        };

        await _commandBus.SendAsync(command);
        return Ok(true);
    }

    /// <summary>
    /// 获取用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>用户信息</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(Guid id)
    {
        var user = await _userService.GetUserAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    /// <summary>
    /// 获取所有用户
    /// </summary>
    /// <returns>用户列表</returns>
    /// <response code="200">成功获取用户列表</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        // 这里简化处理，实际应该从数据库获取
        var users = new List<User>
        {
            new User { UserId = Guid.NewGuid(), Name = "测试用户1", Email = "test1@example.com", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new User { UserId = Guid.NewGuid(), Name = "测试用户2", Email = "test2@example.com", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        };
        return Ok(users);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> DeleteUser(Guid id)
    {
        var success = await _userService.DeleteUserAsync(id);
        return Ok(success);
    }

    /// <summary>
    /// 发布测试事件
    /// </summary>
    /// <returns>结果</returns>
    [HttpPost("test-event")]
    public async Task<ActionResult> PublishTestEvent()
    {
        var testEvent = new UserCreatedEvent
        {
            UserId = Guid.NewGuid(),
            UserName = "测试用户",
            Email = "test@example.com"
        };

        await _eventBus.PublishAsync(testEvent);
        return Ok("事件已发布");
    }
}

/// <summary>
/// 创建用户请求
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required(ErrorMessage = "用户名不能为空")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "用户名长度必须在2-50个字符之间")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    [Required(ErrorMessage = "邮箱不能为空")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "密码不能为空")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "密码长度必须在6-100个字符之间")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// 更新用户请求
/// </summary>
public class UpdateUserRequest
{
    /// <summary>
    /// 用户名
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = string.Empty;
}
