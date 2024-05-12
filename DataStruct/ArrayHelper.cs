using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStruct
{
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
}
