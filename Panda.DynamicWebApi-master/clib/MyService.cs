using System.Runtime.CompilerServices;
using ICliService;

namespace clib
{
    [MyServiceAttribute.My]
    public class MyService
    {
        private readonly ICusService _cliservice;
        public MyService(ICusService cliservice)
        {
            _cliservice = cliservice;
        }
        public string GetStr()
        {
            return "x";
        }

        public void Add(Pars pars)
        {
            _cliservice.Query(pars.Name);
        }
    }

    public class Pars
    {
        public string Name { get; set; }
    }
}
