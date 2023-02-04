namespace AspNetCoreAOP
{
    public class TestOperatorDbBusiness
    {
        [TransactionInterceptor]
        public async ValueTask Add()
        {
            //TODO事务操作
        }
    }
}
