using AspectCore.DynamicProxy;

namespace AspNetCoreAOP
{
    public class TransactionInterceptorAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var dbcontext = context.ServiceProvider.GetService<CommonDbContext>();
            if (dbcontext.Database.CurrentTransaction != null)
            {
                await dbcontext.Database.BeginTransactionAsync();
                try
                {
                    await next(context);
                    await dbcontext.Database.CommitTransactionAsync();
                }catch(Exception ex)
                {
                   await dbcontext.Database.RollbackTransactionAsync();
                    throw ex;
                }
            }
            else
            {
                await next(context);
            }
        }
    }
}
