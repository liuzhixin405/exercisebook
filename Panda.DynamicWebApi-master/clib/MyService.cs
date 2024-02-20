using System.Runtime.CompilerServices;
using Panda.DynamicWebApi.Attributes;

namespace clib
{
    [MyServiceAttribute.My]
    public class MyService
    {
        public string GetStr()
        {
            return "x";
        }

        public void Add(Pars pars)
        {

        }
    }

    public class Pars
    {
        public string Name { get; set; }
    }
}
