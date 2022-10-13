using System.Diagnostics;

namespace LiveScore_ES.Framework.Utils
{
    public static class IniExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="lowerBound">下限</param>
        /// <param name="step">分基</param>
        /// <returns></returns>
        public static int Decrement(this int number,int lowerBound=0,int step = 1)
        {
            var n = number - step;
            return n <lowerBound ? lowerBound : n;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="upperBound">上限</param>
        /// <param name="step">分基 默认每次加一分</param>
        /// <returns></returns>
        public static int Increment(this int number ,int upperBound=100,int step = 1)
        {
            var n = number + step;
            return n > upperBound ? upperBound : n;
        }
    }
}
