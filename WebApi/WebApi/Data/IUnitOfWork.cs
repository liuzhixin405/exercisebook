namespace WebApi.Data
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync();
    }
}
