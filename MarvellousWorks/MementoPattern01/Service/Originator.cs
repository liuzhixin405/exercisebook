using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MementoPattern01.Service
{
    public class Originator:OriginatorBase<Position,Memento>
    {
        public Position Current => base.state;

        public void UpdateX(int x)
        {
            base.state.X = x;
        }
        public void DecX(int x)
        {
            base.state.X--;
        }
        public void IncY(int y)
        {
            base.state.Y++;
        }
        public void UpdateY(int y)
        {
            base.state.Y = y;
        }
        public void DecY(int y)
        {
            base.state.Y--;
        }
        public void IncX(int x)
        {
            base.state.X++;
        }
    }
}
