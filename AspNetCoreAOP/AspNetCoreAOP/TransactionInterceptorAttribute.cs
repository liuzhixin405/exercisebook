using AspectCore.DynamicProxy;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AspNetCoreAOP
{
    public class TransactionInterceptorAttribute : AbstractInterceptorAttribute
    {
        //public override async Task Invoke(AspectContext context, AspectDelegate next)
        //{
        //    var dbcontext = context.ServiceProvider.GetService<CommonDbContext>();
        //    if (dbcontext.Database.CurrentTransaction != null)
        //    {
        //        await dbcontext.Database.BeginTransactionAsync();
        //        try
        //        {
        //            await next(context);
        //            await dbcontext.Database.CommitTransactionAsync();
        //        }catch(Exception ex)
        //        {
        //           await dbcontext.Database.RollbackTransactionAsync();
        //            throw ex;
        //        }
        //    }
        //    else
        //    {
        //        await next(context);
        //    }
        //}//一个context

        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            var dbcontext = context.ServiceProvider.GetService<CommonDbContext>();
            var dbcontextNext = context.ServiceProvider.GetService<NextDbContext>();
            var transactionManager = dbcontext.Database.GetService<IDbContextTransactionManager>();
            var transaction = await transactionManager.BeginTransactionAsync();

            if (transaction != null)
            {
                await dbcontext.Database.BeginTransactionAsync();
                try
                {
                    await next(context);
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw ex;
                }
            }
            else
            {
                await next(context);
            }
        }//多个context
    }
}
