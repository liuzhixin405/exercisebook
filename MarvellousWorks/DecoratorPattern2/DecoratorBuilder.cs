using DecoratorPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorPattern2
{
    public class DecoratorBuilder
    {
        private DecoratorAssembly assembly = new DecoratorAssembly();

        public IText BuilUp(IText target)
        {
            if (target == null) throw new ArgumentNullException("target");
            IList<Type> types = assembly[target.GetType()];
            if(types!=null&&types.Count > 0)
            
                foreach (Type type in types)
                {
                    target = (IText)Activator.CreateInstance(type, target);
                }
                return target;
        }
    }
}
