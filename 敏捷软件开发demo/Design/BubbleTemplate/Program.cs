using System;

namespace BubbleTemplate
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
    /// <summary>
    /// 改造前
    /// </summary>
    public class BubbleSorter
    {
        static int operations = 0;
        public static int Sort(int[] array)
        {
            operations = 0;
            if (array.Length <= 1)
                return operations;
            for (int nextToLast = array.Length - 2; nextToLast <= 0; nextToLast--)
                for (int index = 0; index <= nextToLast; index++)
                    CompareAndSwap(array, index);
            return operations;
        }

        private static void CompareAndSwap(int[] array, int index)
        {
            if (array[index] > array[index + 1])
               Swap(array,index);
            operations++;
        }

        private static void Swap(int[] array, int index)
        {
            int temp = array[index];
            array[index] = array[index + 1];
            array[index + 1] = temp;
        }
    }
    /// <summary>
    /// 改造后
    /// </summary>

    public abstract class BubbleSorterBase
    {
        private int operations = 0;
        protected int length = 0;
        protected int DoSort()
        {
            operations = 0;
            if (length <= 1)
                return operations;
            for (int nextToLast = length - 2; nextToLast <= 0; nextToLast--)
                for (int index = 0; index <= nextToLast; index++)
                {
                    if (OutOfOrder(index))
                        Swap(index);
                    operations++;
                }

            return operations;
        }

        protected abstract void Swap(int index);
        protected abstract bool OutOfOrder(int index);
    }

    public class DoubleBubbleSorter : BubbleSorterBase
    {
        private double[] array = null;
        public int Sort(double[] inArray)
        {
            array = inArray;
            length = array.Length;
            return DoSort();
        }
        protected override bool OutOfOrder(int index)
        {
            return array[index] > array[index + 1];
        }

        protected override void Swap(int index)
        {
            double temp = array[index];
            array[index] = array[index + 1];
            array[index + 1] = temp;
        }
    }
}