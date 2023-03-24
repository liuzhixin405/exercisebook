using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    using Metalama.Framework.Aspects;

    public class LogAttribute : OverrideMethodAspect
    {
        public override dynamic? OverrideMethod()
        {
            Console.WriteLine(meta.Target.Method.ToDisplayString() + " started.");

            try
            {
                var result = meta.Proceed();

                Console.WriteLine(meta.Target.Method.ToDisplayString() + " succeeded.");

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(meta.Target.Method.ToDisplayString() + " failed: " + e.Message);

                throw;
            }
        }
    }
}
