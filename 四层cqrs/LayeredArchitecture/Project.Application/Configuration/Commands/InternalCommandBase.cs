using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Configuration.Commands
{
    public abstract class InternalCommandBase:ICommand
    {
        public Guid Id { get; }
        protected InternalCommandBase(Guid guid) { Id = guid; }
    }
    public abstract class InternalCommandBase<TResult>:ICommand<TResult>
    {
        public Guid Id { get; }
        public InternalCommandBase()
        {
            this.Id = Guid.NewGuid();
        }
        protected InternalCommandBase(Guid id) => this.Id = id;
    }
}
