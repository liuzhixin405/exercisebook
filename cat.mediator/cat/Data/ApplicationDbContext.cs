using cat.Extensions.MediatorExtensions;
using cat.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace cat.Data
{
    public sealed class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
        {
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,IMediator mediator):base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        private readonly IMediator _mediator;
        public DbSet<Contract> Contracts { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _mediator.DispatchDomainEventsAsync(this);
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
