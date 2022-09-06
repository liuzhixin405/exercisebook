using System;

namespace VisitorDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Director director = new Director(new VisitorFoo());
            director.Accept(new Circle());
            Console.Read();
        }
    }
    internal abstract class Shape { internal abstract void Show(); internal abstract void Accept(ShapeVisitor visitor); }
    internal class Circle : Shape
    {
        internal override void Accept(ShapeVisitor visitor)
        {
            visitor.Visit(this);
        }

        internal override void Show()
        {
            Console.WriteLine("Circle");
        }
    }
    internal class Line : Shape
    {
        internal override void Accept(ShapeVisitor visitor)
        {
            visitor.Visit(this);
        }

        internal override void Show()
        {
            Console.WriteLine("Line");
        }
    }
    internal class Rectangle : Shape
    {
        internal override void Accept(ShapeVisitor visitor)
        {
            visitor.Visit(this);
        }

        internal override void Show()
        {
            Console.WriteLine("Rectangle");
        }
    }
    internal abstract class ShapeVisitor
    {
        internal abstract void Visit(Circle circle);
        internal abstract void Visit(Line line);
        internal abstract void Visit(Rectangle rectangle);
    }
    internal class VisitorBar : ShapeVisitor
    {
        internal override void Visit(Circle circle)
        {
            Console.WriteLine("bar扩展circle前");
            circle.Show();
            Console.WriteLine("bar扩展circle后");
        }

        internal override void Visit(Line line)
        {
            Console.WriteLine("bar扩展line前");
            line.Show();
            Console.WriteLine("bar扩展line后");
        }

        internal override void Visit(Rectangle rectangle)
        {
            Console.WriteLine("bar扩展rectangle前");
            rectangle.Show();
            Console.WriteLine("bar扩展rectangle后");
        }
    }
    internal class VisitorFoo : ShapeVisitor
    {
        internal override void Visit(Circle circle)
        {
            Console.WriteLine("foo扩展circle前");
            circle.Show();
            Console.WriteLine("foo扩展circle后");
        }

        internal override void Visit(Line line)
        {
            Console.WriteLine("foo扩展line前");
            line.Show();
            Console.WriteLine("foo扩展line后");
        }

        internal override void Visit(Rectangle rectangle)
        {
            Console.WriteLine("foo扩展rectangle前");
            rectangle.Show();
            Console.WriteLine("foo扩展rectangle后");
        }
    }
    /// <summary>
    /// 管理层
    /// </summary>
    internal class Director
    {
        ShapeVisitor _visitor;
        public Director(ShapeVisitor visitor)
        {
            _visitor = visitor;
        }

        public void Accept(Shape shape)
        {
            shape.Accept(_visitor);
        }
    }
}
