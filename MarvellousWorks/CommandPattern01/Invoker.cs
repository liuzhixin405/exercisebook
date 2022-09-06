using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandPattern01
{
    public class Invoker
    {
        private IList<ICommand> _commands = new List<ICommand>();
        public void AddCommand(ICommand command) => _commands.Add(command);

        public void Run()
        {
            foreach (var command in _commands)
            {
                command.Execute();
            }
        }
    }
}
