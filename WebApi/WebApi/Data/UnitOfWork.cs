namespace WebApi.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbFactory _factory;
        public UnitOfWork(DbFactory factory)
        {
            _factory= factory;
        }
        public async Task<int> CommitAsync()
        {
           return await _factory.DbContext.SaveChangesAsync();
        }
    }
}
