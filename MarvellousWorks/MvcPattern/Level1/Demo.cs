using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcPattern.Level1
{
    public class Demo
    {
        private const int Max = 10;
        private int[] Generate()
        {
            Random random = new Random();
            int[] data = new int[Max];
            for (int i = 0; i < Max; i++)
            {
                data[i] = random.Next() % 1023;
            }
            return data;
        }
        public void PrintData()
        {
            string result = string.Join(",", Array.ConvertAll<int, string>(Generate(), x => x.ToString()));
            Trace.WriteLine(result);
            Console.WriteLine(result);
        }
    }
}
