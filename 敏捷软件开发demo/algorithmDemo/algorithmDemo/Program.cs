using System.Numerics;

namespace algorithmDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var arr = new int[] { 6, 3, 1, 5, 7, 2 };
            SelectionSort(arr);
            Console.WriteLine($"排序结果,{System.Text.Json.JsonSerializer.Serialize(arr)}");
        }
        public static void BubbleSort(int[] arr)
        {
            var len = arr.Length - 1;
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len - i; j++)
                {
                    if (arr[j] > arr[j + 1])
                    {
                        Swap(ref arr[j], ref arr[j + 1]);
                    }
                }
            }
        }
        private static void Swap(ref int x,ref int y)
        {
            var temp = x;
            x = y;
            y = temp;
        }
        
        public static void SelectionSort(int[] arr)
        {
            var len = arr.Length;
            int minIndex, temp;
            for (int i = 0; i < len-1; i++)
            {
                minIndex = i;
                for (int j = i+1; j < len; j++)
                {
                    if (arr[j] < arr[minIndex])
                        minIndex = j;
                }

                temp = arr[i];
                arr[i] = arr[minIndex];
                arr[minIndex] = temp;
            }
        }
    }
}