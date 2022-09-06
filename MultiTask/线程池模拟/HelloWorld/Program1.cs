using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelloWorld
{

    class Program1
    {
        /// <summary>
        /// 版本一
        /// </summary>
        /// <param name="lists"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        static List<object[]> ThreeProcessPartial(List<int> lists, int len)
        {
            int count = lists.Count; //已知数组个数
            List<object[]> ints = new List<object[]>();
            //取余分组
            var arrCount = count % len != 0 ? count / len + 1 : count / len;

            object[] obj0 = new object[arrCount];
            object[] obj1 = new object[arrCount];
            object[] obj2 = new object[arrCount];
            int k = 0;
            while (count > 0)        //10--
            {
                if (count % 3 == 0) //0
                {
                    obj0[k] = "test" + count;
                }
                else if (count % 3 == 1)    //1
                {
                    obj1[k] = "test" + count;
                }
                else if (count % 3 == 2) //2
                {
                    obj2[k] = "test" + count;
                    k++;
                }
                count--;
            }
            ints.Add(obj0);
            ints.Add(obj1);
            ints.Add(obj2);
            return ints;
        }
        /// <summary>
        /// 版本二
        /// </summary>
        /// <param name="list"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        static IEnumerable<object> GetResult(List<int> list,Func<int,bool> func)
        {
            List<object> result = new List<object>();
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                if (func(i))
                {
                    result.Add(list[i]);
                }
            }
            return result;
        }

        static void One_Main(string[] args)
        {
            //版本二
            {
                List<int> list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                List<object[]> result = new List<object[]>();
                 var count = list.Count;
                var r1 = GetResult(list,(x)=> x % 3 == 0).ToArray();
                var r2 = GetResult(list, (x) => x % 3 == 1).ToArray();
                var r3 = GetResult(list, (x) => x % 3 == 2).ToArray();
                result.Add(r2); result.Add(r1); result.Add(r3);
                ShowResult(result);
            }
            //版本一
            {
                List<int> list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                var result = ThreeProcessPartial(list, 3);
                ShowResult(result);
            }
            //未完待续       
            Console.Read();
        }
        /// <summary>
        /// 展示
        /// </summary>
        /// <param name="result"></param>
        private static void ShowResult(List<object[]> result)
        {
            int i = 1;
            foreach (object[] item in result)
            {
                Console.WriteLine($"..........第{i++}组.........");
                foreach (var sub in item)
                {
                    Console.WriteLine(sub);
                }
            }
        }
    }
}
