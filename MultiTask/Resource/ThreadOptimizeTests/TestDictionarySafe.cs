using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ThreadOptimizeTests
{
    /// <summary>
    /// �����ֵ伯�ϵĶ��߳��̰߳�ȫ
    /// </summary>
    [TestClass]
    public class TestDictionarySafe
    {
        private static IDictionary<string, string> Dictionaries { get; set; } = new Dictionary<string, string>();
        private static IDictionary<string, string> ConcurrentDictionaries { get; set; } = new ConcurrentDictionary<string, string>();

        [TestMethod]
        public void TestDictionarySafeMethod()
        {
            Stopwatch sw = new Stopwatch(); //����ͳ��ʱ�����ĵ�

            sw.Restart();
            Task t1 = Task.Factory.StartNew(() => AddDictionaries(1));
            Task t2 = Task.Factory.StartNew(() => AddDictionaries(2));
            Task t3 = Task.Factory.StartNew(() => AddDictionaries(3));

            Task.WaitAll(t1, t2, t3); //ͬ��ִ��
            sw.Stop();
            Console.WriteLine("Dictionaries ��ǰ������Ϊ�� {0}", Dictionaries.Count);
            Console.WriteLine("Dictionaries ִ��ʱ��Ϊ�� {0} ms", sw.ElapsedMilliseconds);


            sw.Restart();
            Task t21 = Task.Factory.StartNew(() => AddConcurrentDictionaries(1));
            Task t22 = Task.Factory.StartNew(() => AddConcurrentDictionaries(2));
            Task t23 = Task.Factory.StartNew(() => AddConcurrentDictionaries(3));

            Task.WaitAll(t21, t22, t23); //ͬ��ִ��
            sw.Stop();
            Console.WriteLine("ConcurrentDictionaries ��ǰ������Ϊ�� {0}", ConcurrentDictionaries.Count);
            Console.WriteLine("ConcurrentDictionaries ִ��ʱ��Ϊ�� {0} ms", sw.ElapsedMilliseconds);


            Assert.IsTrue(sw.ElapsedMilliseconds > 0);
        }

        static void AddDictionaries(int index)
        {
            Parallel.For(0, 1000000, (i) =>
            {
                var key = $"key-{index}-{i}";
                var value = $"value-{index}-{i}";

                // �������ᱨ��
                lock (Dictionaries)
                {
                    Dictionaries.Add(key, value);
                }
            });
        }

        static void AddConcurrentDictionaries(int index)
        {
            Parallel.For(0, 1000000, (i) =>
            {
                var key = $"key-{index}-{i}";
                var value = $"value-{index}-{i}";

                // �������
                ConcurrentDictionaries.Add(key, value);
            });
        }
    }
}