using System;

namespace 策略模式二
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Dog[] dogs = { new Dog(3), new Dog(4), new Dog(1), new Dog(2) };
            Sorter<Dog>.Sort(dogs, new DogComparetor());
            foreach (var dog in dogs)
            {
                Console.WriteLine(dog.ToString());
            }
        }
    }

    internal interface IComparetor<T>
    {
        int Compare(T x, T y);
    }

    internal class Sorter<T>
    {
        internal static void Sort(T[] arr, IComparetor<T> comparetor)
        {
            for (int i = 0; i < arr.Length - 1; i++)
            {
                int minPos = i;
                for (int j = i + 1; j < arr.Length; j++)
                {
                    minPos = comparetor.Compare(arr[j], arr[minPos]) == -1 ? j : minPos;
                }
                Swap(arr, i, minPos);
            }
        }

        private static void Swap(T[] arr, int i, int minPos)
        {
            T tmp = arr[i];
            arr[i] = arr[minPos];
            arr[minPos] = tmp;
        }
    }

    internal class DogComparetor : IComparetor<Dog>
    {
        public int Compare(Dog x, Dog y)
        {
            if (x.Food > y.Food)
            {
                return 1;
            }
            else if (x.Food < y.Food)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }

    internal class Dog
    {
        int food;
        public Dog(int fod)
        {
            this.food = fod;
        }

        public int Food { get { return food; } set { food = value; } }
        public override string ToString()
        {
            return $"food={food} ";
        }
    }
}
