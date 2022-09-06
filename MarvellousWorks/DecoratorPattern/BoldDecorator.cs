using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorPattern
{
    public class BoldDecorator : DecoratorBase
    {
        public BoldDecorator(IText target):base(target)
        {

        }
        public override string Content => ChangeToBoldFont(target.Content);
        public string ChangeToBoldFont(string content) => $"<b>{content}</b>";
    }
}
