using System;

namespace 装饰器
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Component component = new ConcreateComponent();
            component = new ContrecteDecoratorA(component);
            component = new ContrecteDecoratorB(component); //顺序可以调
            //可以无限叠加新动作进来
            component.Operation();
            
        }
    }

    internal abstract class Component
    {
        internal abstract void Operation();
    }

    internal class ConcreateComponent : Component
    {
        internal override void Operation()
        {
            Console.WriteLine("ConcreateComponent.Operation()");
        }
    }
    internal abstract class Decorator : Component
    {
        /// <summary>
        /// 组合实现 构造函数传的类型只要继承了Component就行,好似递归可以无限极循环 这里存档原始的动作
        /// </summary>
        protected Component _component;
        internal Decorator(Component component)
        {
            _component = component;
        }
        /// <summary>
        /// 继承来的,规范接口而已。没有具体的动作
        /// </summary>
        internal override void Operation()
        {
            if (_component != null)
            {
                _component.Operation();
            }
        }
    }

    internal class ContrecteDecoratorA : Decorator
    {
        internal ContrecteDecoratorA(Component component) : base(component)
        {
        }

        internal override void Operation()
        {
            Console.WriteLine("A执行前动作");
            base.Operation();
            Console.WriteLine("A执行后收尾动作");
        }
    }
    internal class ContrecteDecoratorB : Decorator
    {
        internal ContrecteDecoratorB(Component component) : base(component)
        {
        }

        internal override void Operation()
        {
            Console.WriteLine("B执行前动作");
            base.Operation();
            Console.WriteLine("B执行后收尾动作");
        }
    }
}
