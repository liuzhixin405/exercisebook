using webapi.Dals.Impls;
using webapi.Exceptions;

namespace webapi.Services.Impl
{
    public abstract class BqBaseService<Dal> where Dal:class
    {
        protected abstract Dal CreateInstance(string config);
        protected readonly Dal instance;

        public BqBaseService(IConfiguration configuration)
        {
           var _connectionString = configuration["ConnectionStrings"] ?? throw new SystemErrorException("mysql配置链接有误");
        
            instance = CreateInstance(_connectionString);
        }
    }
}
