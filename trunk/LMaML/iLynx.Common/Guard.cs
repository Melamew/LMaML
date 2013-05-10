//using System;

//namespace iLynx.Common
//{
//    /// <summary>
//    /// Guard
//    /// </summary>
//    public static class Guard
//    {
//        /// <summary>
//        /// Determines whether the specified item is null.
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="item">The item.</param>
//        /// <param name="name">The name.</param>
//        /// <exception cref="System.ArgumentNullException"></exception>
//        public static void IsNull<T>(T item, string name)
//        {
//            if (Equals(null, item))
//                throw new ArgumentNullException(name);
//        }

//        /// <summary>
//        /// Determines whether [is null or empty] [the specified STR].
//        /// </summary>
//        /// <param name="str">The STR.</param>
//        /// <param name="name">The name.</param>
//        /// <exception cref="System.ArgumentNullException"></exception>
//        public static void IsNullOrEmpty(string str, string name)
//        {
//            if (string.IsNullOrEmpty(str))
//                throw new ArgumentNullException(str, name);
//        }
//    }
//}
