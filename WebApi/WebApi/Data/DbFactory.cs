namespace WebApi.Data
{
    public class DbFactory:IDisposable
    {
        private bool _disposed;
        private Func<OrderDbContext> _instanceFunc;
        private OrderDbContext _context;
        public  OrderDbContext DbContext => _context ?? (_context = _instanceFunc.Invoke());

        public DbFactory(Func<OrderDbContext> func)
        {
            _instanceFunc = func;
        }
        public void Dispose()
        {
            if (!_disposed && _context != null)
            {
                _disposed = true;
                _context.Dispose();
            }
        }
    }
}
