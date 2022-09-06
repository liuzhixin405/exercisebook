using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using System.Threading;

namespace 模拟双色球.Common
{
    public class RandomHelper
    {
        public int GetRandomNumberDelay(int min,int max)
        {
            Thread.Sleep(this.GetRandomNumber(500,1000));
            return this.GetRandomNumber(min, max);
        }

        private int GetRandomNumber(int min,int max)
        {
            Guid guid = Guid.NewGuid();
            string gGuid = guid.ToString();
            int seed = DateTime.Now.Millisecond;
            for (int i = 0; i < gGuid.Length; i++)
            {
                switch (gGuid[i])
                {
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                    case 'g':
                        seed = seed + 1;
                        break;
                    case 'h':
                    case 'i':
                    case 'j':
                    case 'k':
                    case 'l':
                    case 'm':
                    case 'n':
                        seed = seed + 2;
                        break;
                    case 'o':
                    case 'p':
                    case 'q':
                    case 'r':
                    case 's':
                    case 't':
                        seed = seed + 3;
                        break;
                    case 'u':
                    case 'v':
                    case 'w':
                    case 'x':
                    case 'y':
                    case 'z':
                        seed = seed + 4;
                        break;
                    default:
                        seed = seed + 5;
                        break;
                }
            }
            Random random = new Random(seed);
            return random.Next(min, max);

        }
    }
}
