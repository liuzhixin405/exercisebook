using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogLibrary
{
    public class SubLog : IObserver<DiagnosticListener>
    {
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(DiagnosticListener value)
        {
            //if(value.Name== "MyListener")
            if(value.Name=="webapi")
            value.Subscribe(new LogClient());
            if (value.Name == "MyListener")
            {

            }
        }
    }
}
