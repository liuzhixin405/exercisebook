using System;

namespace LiveScoreEs.Framework.Utils
{
    public static class IntExtensions
    {
        /// <summary>
        /// Decrement a number ensuring it never passes a given lower-bound.
        /// </summary>
        /// <param name="number">Number to process</param>
        /// <param name="lowerBound">Lower bound</param>
        /// <param name="step">Step of the decrement</param>
        /// <returns>Integer</returns>
        public static Int32 Decrement(this Int32 number, Int32 lowerBound = 0, Int32 step = 1)
        {
            var n = number - step;
            return n < lowerBound ? lowerBound : n;
        }

        /// <summary>
        /// Increment a number ensuring it never passes a given upper-bound.
        /// </summary>
        /// <param name="number">Number to process</param>
        /// <param name="upperBound">Upper bound</param>
        /// <param name="step">Step of the increment</param>
        /// <returns>Integer</returns>
        public static Int32 Increment(this Int32 number, Int32 upperBound = 100, Int32 step = 1)
        {
            var n = number + 1;
            return n > upperBound ? upperBound : n;
        } 
    }
}