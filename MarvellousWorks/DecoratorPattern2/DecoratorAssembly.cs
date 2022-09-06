using DecoratorPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorPattern2
{
    public class DecoratorAssembly
    {
        private static IDictionary<Type, IList<Type>> dictionary = new Dictionary<Type, IList<Type>>();

        static DecoratorAssembly()
        {
            IList<Type> types = new List<Type>();
            types.Add(typeof(BoldDecorator));
            types.Add(typeof(ColorDecorator));
            dictionary.Add(typeof(TextObject), types);
        }
        public IList<Type> this[Type type]
        {
            get
            {
                if (type == null) throw new ArgumentNullException("type");
                IList<Type> result;
                return dictionary.TryGetValue(type, out result) ? result : null;
            }
        }
    }
}
