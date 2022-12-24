 using cat.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace cat.DbProvider
{
    public interface IContextProvider
    {
        ApplicationDbContext Get(IMediator mediator);
    }

    public class ContextProvider : IContextProvider
    {
       
        public ApplicationDbContext Get(IMediator mediator=null)
        {
            //var contextBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlServer("").Options;
            var contextBuilder = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase("memory").Options;
            if(mediator != null)
                return new ApplicationDbContext(contextBuilder,mediator);
            else
                return new ApplicationDbContext(contextBuilder);
        }
    }
}
