using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoIdDemo
{
    internal struct Function<T>
    {
        public Func<T, T> run;
        public Function(Func<T, T> fn)
        {
            run = fn;
        }

        public Func<T, T> Then(Function<T> next)
        {
            var runCopy = run;
            return new(x => runCopy(next.run(x)));
        }

        public static Function<T> operator +(Function<T> left,Function<T> right)
        {
            return new(x => left.run(right.run(x)));
        }
    }

    internal static class FunctionExt
    {
        public static Function<T> Then<T>(this Function<T> @this, Function<T> next) => new(x => @this.run(next.run(x)));
    }
}
