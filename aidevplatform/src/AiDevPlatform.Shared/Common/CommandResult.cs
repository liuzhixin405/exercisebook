namespace AiDevPlatform.Shared.Common;

public class CommandResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static CommandResult Success(string? message = null, object? data = null)
    {
        return new CommandResult
        {
            IsSuccess = true,
            Message = message,
            Data = data
        };
    }

    public static CommandResult SuccessResult(string? message = null, object? data = null)
    {
        return new CommandResult
        {
            IsSuccess = true,
            Message = message,
            Data = data
        };
    }

    public static CommandResult Failure(string? message = null, List<string>? errors = null)
    {
        return new CommandResult
        {
            IsSuccess = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }

    public static CommandResult FailureResult(string? message = null, List<string>? errors = null)
    {
        return new CommandResult
        {
            IsSuccess = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }

    public static CommandResult<T> Success<T>(T data, string? message = null)
    {
        return new CommandResult<T>
        {
            IsSuccess = true,
            Message = message,
            Data = data
        };
    }

    public static CommandResult<T> Failure<T>(string? message = null, List<string>? errors = null)
    {
        return new CommandResult<T>
        {
            IsSuccess = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}

public class CommandResult<T> : CommandResult
{
    public new T? Data { get; set; }
}
