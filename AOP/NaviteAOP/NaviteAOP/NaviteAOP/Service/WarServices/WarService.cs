using NaviteAOP.AOP;
using System.Runtime.CompilerServices;

namespace NaviteAOP.Service.WarServices
{
    public class WarService : IWarService
    {
        public IWarService Proxy(IWarService warService)
        {
            return WarDispatch<IWarService>.Create(warService);
        }

        public string WipeOut()
        {
            return "us is over";
        }
    }
}
