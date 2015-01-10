using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Geex.Edit.Common.Tools
{
    /// <summary>
    /// Class providing tools for copying array.
    /// </summary>
    public static class ArrayCopy<T>
    {
        /// <summary>
        /// Makes a copy of the given array.
        /// </summary>
        /// <returns>The copy of the given array.</returns>
        public static T[] Copy(T[] original)
        {
            T[] output = new T[original.Count()];
            for(int i = 0; i < original.Count(); i++)
            {
                output[i] = original[i];
            }
            return output;
        }
        /// <summary>
        /// Fills a given array with the given pattern array.
        /// </summary>
        /// <param name="array">The array which will be filled</param>
        /// <param name="pattern">The array which will fill the first array.</param>
        public static void FillWith(T[] array, T[] pattern)
        {
            int end = Math.Min(array.Count(), pattern.Count());
            for(int i = 0; i < end; i++)
            {
                array[i] = pattern[i];
            }
        }
        /// <summary>
        /// Fills a given array with the given pattern array.
        /// </summary>
        /// <param name="array">The array which will be filled</param>
        /// <param name="pattern">The array which will fill the first array.</param>
        public static void FillWith(List<T> array, List<T> pattern)
        {
            int end = Math.Min(array.Count(), pattern.Count());
            for (int i = 0; i < end; i++)
            {
                array[i] = pattern[i];
            }
        }
    }
}
