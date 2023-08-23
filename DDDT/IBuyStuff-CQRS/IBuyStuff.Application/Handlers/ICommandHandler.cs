using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBuyStuff.Application.Commands;
using IBuyStuff.Application.ViewModels;

namespace IBuyStuff.Application.Handlers
{
    public interface ICommandHandler<in TCommand,out TViewModel> where TCommand:Command where TViewModel : ViewModelBase
    {
        TViewModel Handle(TCommand command);
    }
}
