using Microsoft.EntityFrameworkCore;

namespace chatgptwriteproject.DbFactories
{
    public class DbFactory<T> where T:DbContext
    {
        private bool _disposed;
        private Func<T> _instanceFunc;
        private T _context;
        public T Context => _context ?? (_context = _instanceFunc());
        public DbFactory(Func<T> instance)
        {
            _instanceFunc= instance??throw new ArgumentNullException("dbcontext is null");
        }

        public void Dispose()
        {
            _disposed = true;   
            if(_context!= null)
            {
                _context.Dispose();
            }
        }
    }
}
