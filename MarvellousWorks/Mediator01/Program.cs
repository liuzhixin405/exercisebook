using System.Diagnostics;

namespace Mediator01
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Mediator<int> mediator = new Mediator<int>();
            A a = new A();
            B b = new B();
            C c = new C();
            a.Mediator = mediator;
            b.Mediator = mediator;
            c.Mediator = mediator;
            mediator.Introduce(a, b, c);
            a.Data = 20;
            Trace.WriteLine($"b.data={b.Data},c.data={c.Data}");
            mediator.Introduce(a, b);
            a.Data = 100;
            Trace.WriteLine($"b.data={b.Data},c.data={c.Data}");
        }
    }

    public class A : ColleagueBase<int>
    {
        public override int Data
        {
            get => base.data; set
            {
                data = value;
                base.mediator.Change();
            }
        }
    }
    public class B : ColleagueBase<int> { }
    public class C : ColleagueBase<int> { }
    public interface IMediator<T>
    {
        void Change();
        void Introduce(IColleague<T> provider, IColleague<T> consumer);
        void Introduce(IColleague<T> provider, List<IColleague<T>> conusmers);
        void Introduce(IColleague<T> provider, params IColleague<T>[] conusmers);
    }
    public interface IColleague<T> { T Data { get; set; } IMediator<T> Mediator { get; set; } }

    public abstract class ColleagueBase<T> : IColleague<T>
    {
        protected T data;
        protected IMediator<T> mediator;
        public virtual T Data { get => data; set => data = value; }
        public virtual IMediator<T> Mediator { get => mediator; set => mediator = value; }
    }

    public class Mediator<T> : IMediator<T>
    {
        private IColleague<T> provider;
        private List<IColleague<T>> consumers;
        public void Change()
        {
            if (provider != null && consumers != null)
                foreach (var consumer in consumers)
                {
                    consumer.Data = provider.Data;
                }
        }

        public void Introduce(IColleague<T> provider, IColleague<T> consumer)
        {
            this.provider = provider;
            var consumers = new List<IColleague<T>>();
            consumers.Add(consumer);
            this.consumers = consumers;
        }

        public void Introduce(IColleague<T> provider, List<IColleague<T>> conusmers)
        {
            this.provider = provider;
            var consumers = new List<IColleague<T>>(conusmers);
            this.consumers = consumers;
        }

        public void Introduce(IColleague<T> provider, params IColleague<T>[] conusmers)
        {
            this.provider = provider;
            var consumers = new List<IColleague<T>>(conusmers);
            this.consumers = consumers;
        }
    }
}