using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorPattern2
{
    public class ColorState : IState
    {
        public Color Color = Color.Yellow;
        public bool Equals(IState newState)
        {
            if(newState == null)return false;
            return ((ColorState)newState).Color == Color;
        }
    }
}
