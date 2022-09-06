using DecoratorPattern2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorPattern
{
    public class ColorDecorator : DecoratorBase
    {
        public ColorDecorator(IText target):base(target)
        {
            base.state = new ColorState();
        }
        public override string Content => AddColorTag(target.Content);
        public string AddColorTag(string content)
        {
            string colorName = ((ColorState)state).Color.Name;
           return $"<{colorName}>{target.Content}</{colorName}>";
        } 
    }
}
