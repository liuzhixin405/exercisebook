
using System.Linq.Expressions;
using System.Net.Http.Headers;

namespace DataStruct;

public class Program
{

    static void Main(string[] args)
    {

        // int[] arr= new int[]{1,2,4,5,6,7,8};
        // Console.WriteLine("operation before:");
        // arr.ToList().ForEach(x=> Console.WriteLine(x));
        //  var newArr = ArrayHelper.AddNew<int>(arr,3,2);
        //     Console.WriteLine("operation after:");
        // newArr.ToList().ForEach(x=> Console.WriteLine(x));
        // 测试用例
        int[] arr = { 1, 2, 4, 5, 6, 7, 8 };

        // 插入到第一个位置
        TestAdd(arr, 0, 0);

        // 插入到中间位置
        TestAdd(arr, 3, 2);

        // 插入到最后一个位置
        TestAdd(arr, 9, arr.Length);

        // 插入到超出范围的位置
        TestAddOutOfRange(arr, 10, arr.Length + 1);


        int[] delArr = new int[] { 1, 2, 3, 4, 5 };
        Console.WriteLine("delete before:");
        delArr.ToList().ForEach(x => Console.WriteLine(x));
        var resultDelArr = ArrayHelper.Delete<int>(delArr, 1);
        Console.WriteLine("delete after:");
        resultDelArr.ToList().ForEach(x => Console.WriteLine(x));
    }

    static void TestAdd<T>(T[] arr, T item, int index)
    {
        try
        {
            var newArr = ArrayHelper.Add(arr, item, index);
            Console.WriteLine($"在索引 {index} 处插入元素 {item} 后的数组：");
            foreach (var element in newArr)
            {
                Console.Write(element + " ");
            }
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"插入元素时发生异常：{ex.Message}");
        }
    }

    static void TestAddOutOfRange<T>(T[] arr, T item, int index)
    {
        try
        {
            var newArr = ArrayHelper.Add(arr, item, index);
            Console.WriteLine($"在索引 {index} 处插入元素 {item} 后的数组：");
            foreach (var element in newArr)
            {
                Console.Write(element + " ");
            }
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"插入元素时发生异常：{ex.Message}");
        }
    }

}

public class ArrayHelper
{

    public static T[] Add<T>(T[] arr, T item, int index)
    {
        if (index > arr.Length)
        {
            throw new Exception("数组越界");
        }

        T[] newArr = new T[arr.Length + 1];
        Array.Copy(arr, newArr, arr.Length);

        for (int i = arr.Length - 1; i >= index; i--)
        {
            newArr[i + 1] = newArr[i];
        }
        newArr[index] = item;

        return newArr;
    }

    public static T[] AddNew<T>(T[] arr, T item, int index)
    {
        if (index > arr.Length)
        {
            throw new Exception("数组越界");
        }

        T[] newArr = new T[arr.Length + 1];
        Array.Copy(arr, newArr, index);
        newArr[index] = item;
        Array.Copy(arr, index, newArr, index + 1, arr.Length - index);
        return newArr;
    }

    public static T[] Delete<T>(T[] arr, int index)
    {
        if (index >= arr.Length || index < 0)
        {
            throw new Exception("数组越界");
        }

        for (int i = index; i < arr.Length - 1; i++)
        {
            arr[i] = arr[i + 1];
        }

        T[] newArr = new T[arr.Length - 1];
        Array.Copy(arr, newArr, newArr.Length);
        return newArr;
    }
    public static T[] DeleteNew<T>(T[] arr, int index)
    {
        if (index >= arr.Length || index < 0)
        {
            throw new Exception("数组越界");
        }

        T[] newArr = new T[arr.Length - 1];

        // 复制索引之前的元素
        if (index > 0)
        {
            Array.Copy(arr, 0, newArr, 0, index);
        }

        // 复制索引之后的元素
        if (index < arr.Length - 1)
        {
            Array.Copy(arr, index + 1, newArr, index, arr.Length - index - 1);
        }

        return newArr;
    }

}