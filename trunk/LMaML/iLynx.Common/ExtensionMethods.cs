using System;
using System.Collections.Generic;
using System.Linq;

namespace iLynx.Common
{
    /// <summary>
    /// A few general Extension Methods
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets a string representation of the specified <see name="IEnumerable{byte}"/> using the specified <paramref name="splitter"/> as a "splitter"
        /// </summary>
        /// <param name="val">The <see cref="IEnumerable{T}"/> to stringify</param>
        /// <param name="splitter">The splitter to use between bytes</param>
        /// <returns></returns>
        public static string ToString(this IEnumerable<byte> val, string splitter)
        {
            var ret = val.Aggregate(string.Empty, (current, v) => current + (v.ToString("X2") + splitter));
            if (ret.EndsWith(splitter))
                ret = ret.Remove(ret.LastIndexOf(splitter, StringComparison.InvariantCulture), splitter.Length);
            return ret;
        }

        /// <summary>
        /// Combines to string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val">The val.</param>
        /// <returns></returns>
        public static string CombineToString<T>(this IEnumerable<T> val)
        {
            return val.Aggregate(string.Empty, (s, arg2) => s + arg2.ToString());
        }

        /// <summary>
        /// Determines whether the specified value is within the specified range (Inclusively!).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>
        ///   <c>true</c> if [is in range] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInRange(this short value, short min, short max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Determines whether the specified value is within the specified range (Inclusively!).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>
        ///   <c>true</c> if [is in range] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInRange(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Determines whether the specified value is within the specified range (Inclusively!).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>
        ///   <c>true</c> if [is in range] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInRange(this long value, long min, long max)
        {
            return value >= min && value <= max;
        }
        /// <summary>
        /// Determines whether the specified value is within the specified range (Inclusively!).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>
        ///   <c>true</c> if [is in range] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInRange(this uint value, uint min, uint max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Determines whether the specified value is within the specified range (Inclusively!).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>
        ///   <c>true</c> if [is in range] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInRange(this ushort value, ushort min, ushort max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Determines whether the specified value is within the specified range (Inclusively!).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>
        ///   <c>true</c> if [is in range] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInRange(this ulong value, ulong min, ulong max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Determines whether [is power of two] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [is power of two] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPowerOfTwo(this uint value)
        {
            return (value != 0) && ((value & (value - 1)) == 0);
        }

        /// <summary>
        /// Determines whether [is power of two] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [is power of two] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPowerOfTwo(this ulong value)
        {
            return (value != 0) && ((value & (value - 1)) == 0);
        }

        /// <summary>
        /// Determines whether [is power of two] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [is power of two] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPowerOfTwo(this ushort value)
        {
            return (value != 0) && ((value & (value - 1)) == 0);
        }

        /// <summary>
        /// Determines whether [is power of two] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [is power of two] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPowerOfTwo(this int value)
        {
            return (value > 0) && ((value & (value - 1)) == 0);
        }

        /// <summary>
        /// Determines whether [is power of two] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [is power of two] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPowerOfTwo(this long value)
        {
            return (value > 0) && ((value & (value - 1)) == 0);
        }

        /// <summary>
        /// Determines whether [is power of two] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [is power of two] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPowerOfTwo(this short value)
        {
            return (value > 0) && ((value & (value - 1)) == 0);
        }

        /// <summary>
        /// Normalizes the specified arr.
        /// </summary>
        /// <param name="arr">The arr.</param>
        public static void Normalize(this float[] arr)
        {
            var max = float.MinValue;
            // ReSharper disable LoopCanBeConvertedToQuery // Faster like this, ffs
            // ReSharper disable ForCanBeConvertedToForeach // Faster like this, ffs
            // TODO: Maybe find a way to make this go faster?
            for (var i = 0; i < arr.Length; ++i)
                max = arr[i] > max ? arr[i] : max;
            for (var i = 0; i < arr.Length; ++i)
                arr[i] /= max;
            // ReSharper restore ForCanBeConvertedToForeach
            // ReSharper restore LoopCanBeConvertedToQuery
        }

        /// <summary>
        /// Transforms the specified arr.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr">The arr.</param>
        /// <param name="func">The func.</param>
        public static void Transform<T>(this T[] arr, Func<T, T> func)
        {
            for (var i = 0; i < arr.Length; ++i)
                arr[i] = func(arr[i]);
        }

        /// <summary>
        /// Normalizes the specified arr.
        /// </summary>
        /// <param name="arr">The arr.</param>
        public static void Normalize(this double[] arr)
        {
            var max = double.MinValue;
            // ReSharper disable LoopCanBeConvertedToQuery // Faster like this, ffs
            // ReSharper disable ForCanBeConvertedToForeach // Faster like this, ffs
            // TODO: Maybe find a way to make this go faster?
            for (var i = 0; i < arr.Length; ++i)
                max = arr[i] > max ? arr[i] : max;
            for (var i = 0; i < arr.Length; ++i)
                arr[i] /= max;
            // ReSharper restore ForCanBeConvertedToForeach
            // ReSharper restore LoopCanBeConvertedToQuery
        }

        /// <summary>
        /// Removes the range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="range">The range.</param>
        public static void RemoveRange<T>(this ICollection<T> source, IEnumerable<T> range)
        {
            foreach (var t in range)
                source.Remove(t);
        }
    }
}