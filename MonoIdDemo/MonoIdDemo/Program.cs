using MonoIdDemo;

namespace MonoIdDemo
{
    internal class Program
    {
        static void Main(String[] args)
        {
            var add_sub_1= add5+(sub3+add10);
            add_sub_1.run(5);
            //....
        }
        static Function<int> identity = new Function<int>(a => a);
        static Function<int> add5 = new(a => a + 5);
        static Function<int> add10 = new(Add10);

        static Function<int> sub3=new(a => a - 3);
        static Function<int> mul3 = new(a => a * 3);

        static Function<int> mul5 = new(a => a * 5);
        static Function<int> div7 = new(a => a / 7);

        static int Add10(int a) => a+10;
    }
}

