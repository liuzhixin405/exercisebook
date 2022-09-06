using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorPattern
{
    public class BlockAllDecorator : DecoratorBase
    {
        public BlockAllDecorator(IText target):base(target)
        {

        }
        public override string Content => string.Empty;
    }
}
