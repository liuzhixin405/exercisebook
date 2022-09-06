using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLibrary.Commands
{
    public record InsertPersonCommand(string firstName,string lastName):IRequest<InsertPersonModel>
    {

    }

    public record InsertPersonModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public InsertPersonModel(string firstName,string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
