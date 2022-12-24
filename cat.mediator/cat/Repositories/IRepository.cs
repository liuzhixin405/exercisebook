using cat.Data;
using cat.DbProvider;
using cat.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace cat.Repositories
{
    public interface IRepository
    {
        ValueTask<IEnumerable<Contract>> GetAll(Expression<Func<Contract, bool>> where);
        
        ValueTask Add(Contract contract);
        ValueTask Update(Contract contract);
        ValueTask Delete(Contract contract);

    }

    public class Repository : IRepository
    {
        private readonly ApplicationDbContext _context;
       
        public Repository(IContextProvider provider,IMediator mediator)
        {
           
            _context = provider.Get(mediator);
        }
        public async ValueTask Add(Contract contract)
        {
           await _context.AddAsync(contract);
            await _context.SaveChangesAsync();
        }

        public async ValueTask Delete(Contract contract)
        {
             _context.Remove(contract);
            await _context.SaveChangesAsync();
        }

        public async ValueTask<IEnumerable<Contract>> GetAll(Expression<Func<Contract, bool>> where)
        {
            return await _context.Contracts.Where(where).ToListAsync();
        }

        public async ValueTask Update(Contract contract)
        {
            _context.Set<Contract>().Entry(contract).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
