using DecoratorPattern2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecoratorPattern
{
    public interface IDecorator:IText
    {
        IState State { get; set; }
        void Refresh<T>(IState newState) where T:IDecorator;
    }
    public abstract class DecoratorBase : IDecorator
    {
        protected IText target;
        public DecoratorBase(IText target)
        {
            this.target = target;
        }

        public abstract string Content { get; }
        protected IState state;
        public IState State { get => state; set => this.state = value; }

        public void Refresh<T>(IState newState) where T : IDecorator
        {
            if (this.GetType() == typeof(T))
            {
                if (newState == null)
                    state = null;
                if(state!=null && !state.Equals(newState))
                    state = newState;
                return;
            }
            if (target != null)
                ((IDecorator)target).Refresh<T>(newState);
        }
    }
}
