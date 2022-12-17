using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Configuration.Commands
{
    public class CommandBase:ICommand
    {
        public Guid Id { get; }

        public CommandBase()
        {
            this.Id = Guid.NewGuid();
        }

        protected CommandBase(Guid id)
        {
            this.Id = id;
        }
    }
    public abstract class CommandBase<TResult> : ICommand<TResult>
    {
        public Guid Id { get; }
        protected CommandBase()
        {
            this.Id = Guid.NewGuid();
        }

        protected CommandBase(Guid id)
        {
            this.Id = id;
        }
    }
}
