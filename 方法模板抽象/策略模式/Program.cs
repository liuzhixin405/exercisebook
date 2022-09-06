using System;

namespace 策略模式
{
    /// <summary>
    /// 排序 泛型 无策略模式
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            #region cat
            Cat[] cats = new Cat[5] { new Cat(4, 4), new Cat(2, 2), new Cat(3, 3), new Cat(8, 8), new Cat(7, 7) };
            Sorter.Sort(cats);
            foreach (var cat in cats)
            {
                Console.WriteLine(cat.ToString());
            }
            #endregion

            #region dog
            Dog[] dogs = new Dog[] { new Dog(3), new Dog(1), new Dog(9), new Dog(5) };
            SorterQux.Sort(dogs);
            SorterBar<Dog>.Sort(dogs);
            foreach (var dog in dogs)
            {
                Console.WriteLine(dog.ToString());
            }

            #endregion
        }
    }

    #region cat
    internal class Sorter
    {
        internal static void Sort(IMyComparable[] arr)
        {
            for (int i = 0; i < arr.Length - 1; i++)
            {
                int minPos = i;
                for (int j = i + i; j < arr.Length; j++)
                {
                    minPos = arr[j].Compare(arr[minPos]) == -1 ? j : minPos;
                }
                Swap(arr, i, minPos);
            }
        }


        private static void Swap(IMyComparable[] arr, int i, int j)
        {
            IMyComparable temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }
    }
    internal interface IMyComparable
    {
        internal int Compare(object x);
    }

    internal class Cat : IMyComparable
    {
        private int weight;
        private int height;
        public Cat(int weight, int height)
        {
            this.weight = weight;
            this.height = height;
        }

        int IMyComparable.Compare(object obj)
        {
            Cat other = obj as Cat;
            if (other.height > this.height && other.weight > this.weight)
            {
                return 1;
            }
            else if (other.weight < this.weight && other.height < this.height)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        public override string ToString()
        {
            return $"weigth={weight},height={height}\n";
        }
    }

    #endregion
    #region dog
    /// <summary>
    /// 方法泛型
    /// </summary>
    internal class SorterQux
    {
        internal static void Sort<T>(IMyComparable<T>[] arr) where T : class
        {
            for (int i = 0; i < arr.Length - 1; i++)
            {
                int minPos = i;
                for (int j = i + 1; j < arr.Length; j++)
                {
                    minPos = arr[j].Compare((T)arr[minPos]) == -1 ? j : minPos;
                }
                Swap(arr, i, minPos);
            }
        }

        private static void Swap<T>(T[] arr, int i, int j)
        {
            T temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }
    }
    /// <summary>
    /// 类泛型
    /// </summary>
    internal class SorterBar<T> where T : class
    {
        internal static void Sort(IMyComparable<T>[] arr)
        {
            for (int i = 0; i < arr.Length - 1; i++)
            {
                int minPos = i;
                for (int j = i + 1; j < arr.Length; j++)
                {
                    minPos = arr[j].Compare((T)arr[minPos]) == -1 ? j : minPos;
                }
                Swap(arr, i, minPos);
            }
        }

        private static void Swap(IMyComparable<T>[] arr, int i, int j)
        {
            IMyComparable<T> temp = arr[i];
            arr[i] = arr[j];
            arr[j] = temp;
        }
    }

    internal interface IMyComparable<T> where T : class
    {
        internal int Compare(T x);
    }
    internal class Dog : IMyComparable<Dog>
    {
        int food;
        public Dog(int food)
        {
            this.food = food;
        }
        int IMyComparable<Dog>.Compare(Dog x)
        {
            if (x.food > this.food)
            {
                return -1;
            }
            else if (x.food < this.food)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public override string ToString()
        {
            return $"food={food}";
        }
    }
    #endregion
}
