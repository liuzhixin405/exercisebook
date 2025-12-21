namespace ECommerce.API.Application
{
    // 验证处理器接口
    public interface IValidationHandler<T>
    {
        Task<bool> ValidateAsync(T request);
        string ErrorMessage { get; }
    }
}
