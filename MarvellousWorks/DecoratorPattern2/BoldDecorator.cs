using DecoratorPattern2;
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
            base.state = new BoldState();
        }
        public override string Content => ChangeToBoldFont(target.Content);
        public string ChangeToBoldFont(string content) => ((BoldState)state).IsBold ? $"<b>{content}</b>" : target.Content;
    }
}
