using Contract.Core.Entities;
using Contract.Core.Interfaces;
using Contract.Core.SharedKernel;
using Contract.SharedKernel;
using Contract.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Contract.Infrastructure.Data
{
    public class AppDbContext:DbContext
    {
        private readonly IDomainEventDispatcher _dispatcher;
        public AppDbContext(DbContextOptions<AppDbContext> options,IDomainEventDispatcher dispatcher):base(options)
        {
            _dispatcher = dispatcher;
        }

        public DbSet<Order> Orders =>Set<Order>();
        public DbSet<ToDoItem> ToDoItems =>Set<ToDoItem>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken=default(CancellationToken))
        {
            int result =await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            if (_dispatcher == null) return result;
            var entitiesWithEvents = ChangeTracker.Entries<EntityBase>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToArray();

           await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);
            return result;
        }
        public override int SaveChanges()
        {
            return SaveChangesAsync().GetAwaiter().GetResult();
        }
    }
}
