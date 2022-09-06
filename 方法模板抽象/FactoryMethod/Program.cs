using System;

namespace FactoryMethod
{
    internal class Program
    {
        static void Main(string[] args)
        {
           CarTestFramework framework = new CarTestFramework();
            framework.BuildContext(new HongqiFactory());
            //需要别的车都可以添加新的类，无需现有文件除开program
        }
    }

    public enum Direction { }

    public abstract class AbstractCar
    {
        public abstract void Startup();
        public abstract void Stop();
        public abstract void Turn(Direction direction);
    }

    public class Hongqi : AbstractCar
    {
        public override void Startup()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        public override void Turn(Direction direction)
        {
            throw new NotImplementedException();
        }
    }
    
    public abstract class CarFactory
    {
        public abstract AbstractCar CreateCar();
    }

    public class HongqiFactory : CarFactory
    {
        public override AbstractCar CreateCar()
        {
            return new Hongqi();
        }
    }

    public class CarTestFramework
    {
        public void BuildContext(CarFactory carFactory)
        {
            AbstractCar car = carFactory.CreateCar();
        }
    }
}
