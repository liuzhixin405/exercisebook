using System;
using System.Diagnostics;

namespace 访问者模式
{
    internal class Program
    {
        static void Main(string[] args)
        {
            App app = new App(new MyVisitror());
            app.Preocess(new Circle());

            Console.Read();
        }
    }

    public class App
    {
        ShapeVisitor visitor;
        public App(ShapeVisitor visitor)
        {
            this.visitor = visitor;
        }

        public void Preocess(Shape shape)
        {
            shape.Accept(visitor); //双向反转
            //ShapeVisitor shape = new MyVisitror();
            //Line line = new Line();
            //line.Accept(shape);

            //Rectangle rectangle = new Rectangle();
            //rectangle.Accept(shape);

            //Circle circle = new Circle();
            //circle.Accept(shape);
        }
    }

    public class MyVisitror : ShapeVisitor
    {
        public override void Visit(Circle circle)
        {

        }

        public override void Visit(Line line)
        {

        }

        public override void Visit(Rectangle rectangle)
        {

        }
    }

    /// <summary>
    /// 定义好可能变化的部分,可以随时扩展
    /// </summary>
    public abstract class Shape
    {
        public abstract void Draw();
        public abstract void Accept(ShapeVisitor shapeVisitor);
    }

    public class Rectangle : Shape
    {
        /// <summary>
        /// 扩展的纽带
        /// </summary>
        /// <param name="shapeVisitor"></param>
        public override void Accept(ShapeVisitor shapeVisitor)
        {
            shapeVisitor.Visit(this);
        }

        public override void Draw()
        {
            //基础输出
        }
    }

    public class Circle : Shape
    {
        public override void Accept(ShapeVisitor shapeVisitor)
        {
            shapeVisitor.Visit(this);
        }

        public override void Draw()
        {
            //基础输出
        }
    }
    public class Line : Shape
    {
        public override void Accept(ShapeVisitor shapeVisitor)
        {
            shapeVisitor.Visit(this);
        }

        public override void Draw()
        {
            //基础输出
        }
    }

    /// <summary>
    /// 扩展点
    /// </summary>
    public abstract class ShapeVisitor
    {
        public abstract void Visit(Circle circle);
        public abstract void Visit(Line line);
        public abstract void Visit(Rectangle rectangle);
    }


}
