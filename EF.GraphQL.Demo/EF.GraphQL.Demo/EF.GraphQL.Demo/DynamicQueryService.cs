using Microsoft.EntityFrameworkCore;

namespace EF.GraphQL.Demo
{
    public class DynamicQueryService
    {
        private readonly DbContext _context;

        public DynamicQueryService(DbContext context)
        {
            _context = context;
        }

        public DynamicQueryBuilder<TMain> From<TMain>() where TMain : class
        {
            return new DynamicQueryBuilder<TMain>(_context);
        }
    }
}
