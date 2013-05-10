using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace iLynx.Common
{
    /// <summary>
    /// RuntimeHelper
    /// </summary>
    public static class RuntimeHelper
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns></returns>
        public static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            var expression = propertyExpression.Body as MemberExpression;
            if (null == expression) return String.Empty;
            return expression.Member.MemberType != MemberTypes.Property ? String.Empty : expression.Member.Name;
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">The specified expression does not represent a supported expression type</exception>
        public static string GetMemberName(Expression expression)
        {
            expression.Guard("expression");
            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
                return memberExpression.Member.Name;
            var methodExpression = expression as MethodCallExpression;
            if (null != methodExpression)
                return methodExpression.Method.Name;
            var unary = expression as UnaryExpression;
            if (null != unary)
                return GetMemberName(unary);
            throw new NotSupportedException("The specified expression does not represent a supported expression type");
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <param name="expression">The expresion.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">The specified expression does not represent a supported expression type</exception>
        public static string GetMemberName(UnaryExpression expression)
        {
            expression.Guard("expression");
            var methodCallExpression = expression.Operand as MethodCallExpression;
            if (null != methodCallExpression)
                return methodCallExpression.Method.Name;
            var memberExpression = expression.Operand as MemberExpression;
            if (null != memberExpression)
                return memberExpression.Member.Name;
            throw new NotSupportedException("The specified expression does not represent a supported expression type");
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static string GetMemberName<T>(Expression<Func<T>> expression)
        {
            expression.Guard("expression");
            return GetMemberName(expression.Body);
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public static string GetMemberName<T>(Expression<Action<T>> expression)
        {
            expression.Guard("expression");
            return GetMemberName(expression.Body);
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner">The owner.</param>
        /// <param name="memberExpression">The member expression.</param>
        /// <returns></returns>
        public static string GetMemberName<T>(this T owner, Expression<Func<T>> memberExpression)
        {
            return GetMemberName(memberExpression);
        }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner">The owner.</param>
        /// <param name="memberExpression">The member expression.</param>
        /// <returns></returns>
        public static string GetMemberName<T>(this T owner, Expression<Action<T>> memberExpression)
        {
            return GetMemberName(memberExpression);
        }

        /// <summary>
        /// The lynx namespace
        /// </summary>
        private const string LynxNamespace = "2F9D6016-DA43-4DB9-A94F-78A00265071B";

        /// <summary>
        /// The namespace
        /// </summary>
        public static readonly Guid LynxSpace = new Guid(LynxNamespace);

        /// <summary>
        /// Swaps the byte order.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static byte[] SwapByteOrder(this Guid id)
        {
            var bytes = id.ToByteArray();
            SwapGuidOrder(bytes);
            return bytes;
        }

        /// <summary>
        /// To the network order.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static Guid SwapOrder(this Guid id)
        {
            return new Guid(id.SwapByteOrder());
        }

        /// <summary>
        /// Makes the id long.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public static long MakeIdLong(this Guid id)
        {
            var arr = id.ToByteArray();
            for (var i = 0; i < 8; ++i)
                arr[i] ^= arr[i + 8];
            return BitConverter.ToInt64(arr, 0);
        }

        /// <summary>
        /// Swaps the GUID order.
        /// </summary>
        /// <param name="src">The SRC.</param>
        private static void SwapGuidOrder(byte[] src)
        {
            SwapInt32Endianness(src);
            SwapInt16Endianness(src, 4);
            SwapInt16Endianness(src, 6);
        }

        /// <summary>
        /// Creates an RFC 4122 compliant GUID (v5) for the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="namespaceId">The namespace id.</param>
        /// <returns></returns>
        public static Guid CreateGuidV5(this string name, Guid namespaceId)
        {
            var nameBytes = Encoding.UTF8.GetBytes(name);
            var namespaceBytes = namespaceId.SwapByteOrder();

            byte[] hash;
            using (var algorithm = SHA1.Create())
            {
                algorithm.TransformBlock(namespaceBytes, 0, namespaceBytes.Length, null, 0);
                var count = nameBytes.Length - algorithm.InputBlockSize;
                var offset = 0;
                while ((count -= algorithm.InputBlockSize) > 0)
                {
                    algorithm.TransformBlock(nameBytes, offset, algorithm.InputBlockSize, null, 0);
                    offset += algorithm.InputBlockSize;
                }
                algorithm.TransformFinalBlock(nameBytes, offset, nameBytes.Length - offset);
                hash = algorithm.Hash;
            }

            Buffer.BlockCopy(hash, 0, namespaceBytes, 0, namespaceBytes.Length);
            namespaceBytes[6] = (byte)((namespaceBytes[6] & 0x0F) | (5 << 4)); // set the four most significant bits (bits 12 through 15) of the time_hi_and_version field to the appropriate 4-bit version number from Section 4.1.3 (step 8)
            namespaceBytes[8] = (byte)((namespaceBytes[8] & 0x3F) | 0x80); // set the two most significant bits (bits 6 and 7) of the clock_seq_hi_and_reserved to zero and one, respectively (step 10)
            SwapGuidOrder(namespaceBytes);
            var final = new Guid(namespaceBytes);
#if DEBUG
            Debug.Assert(final == GuidUtility.Create(namespaceId, name));
#endif
            return final;
        }

        #region 16 Bits

        /// <summary>
        /// Swaps the endianness.
        /// </summary>
        /// <param name="array">The array.</param>
        public static void SwapEndianness(this short[] array)
        {
            unsafe
            {
                fixed (void* arr = array)
                    Swap16((byte*)arr, 0, array.Length);
            }
        }

        /// <summary>
        /// Swaps the endianness.
        /// </summary>
        /// <param name="array">The array.</param>
        public static void SwapEndianness(this ushort[] array)
        {
            unsafe
            {
                fixed (void* arr = array)
                    Swap16((byte*)arr, 0, array.Length);
            }
        }

        /// <summary>
        /// Swaps the endianness of a 16 bit integer at the specified byte offset.
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">offset</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">count</exception>
        public static void SwapInt16Endianness(byte[] src, int offset = 0, int count = 1)
        {
            if (offset >= src.Length) throw new ArgumentOutOfRangeException("offset");
            if (offset + (count * 2) >= src.Length) throw new ArgumentOutOfRangeException("count");
            unsafe
            {
                fixed (byte* arr = src)
                    Swap16(arr, offset, count);
            }
        }

        /// <summary>
        /// Swaps the endianness of 16 bit integers starting at the specified byte offset, processing <param name="count"/> full 16 bit integers (Bytes X 2).
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        public static unsafe void Swap16(byte* src, int offset, int count)
        {
            var p = src + offset;
            while (count-- > 0)
            {
                var a = *p;
                ++p;
                *(p - 1) = *p;
                *p = a;
            }
        }
        #endregion

        #region 32 Bits
        /// <summary>
        /// Swaps the endianness.
        /// </summary>
        /// <param name="array">The array.</param>
        public static void SwapEndianness(this int[] array)
        {
            unsafe
            {
                fixed (void* arr = array)
                    Swap32((byte*)arr, 0, array.Length);
            }
        }

        /// <summary>
        /// Swaps the endianness.
        /// </summary>
        /// <param name="array">The array.</param>
        public static void SwapEndianness(this uint[] array)
        {
            unsafe
            {
                fixed (void* arr = array)
                    Swap32((byte*)arr, 0, array.Length);
            }
        }

        /// <summary>
        /// Swaps the endianness of a 32 bit integer at the specified byte offset.
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The number of integers to swap.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">offset</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">count</exception>
        public static void SwapInt32Endianness(byte[] src, int offset = 0, int count = 1)
        {
            if (offset >= src.Length) throw new ArgumentOutOfRangeException("offset");
            if (offset + count * 4 >= src.Length) throw new ArgumentOutOfRangeException("count");
            unsafe
            {
                fixed (byte* arr = src)
                    Swap32(arr, offset, count);
            }
        }

        /// <summary>
        /// Swaps the endianness of 32 bit integers starting at the specified byte offset, processing <param name="count"/> full 32 bit integers (Bytes X 4).
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        public static unsafe void Swap32(byte* src, int offset, int count)
        {
            var p = src + offset;
            while (count-- > 0)
            {
                var a = *p;
                ++p;
                var b = *p;
                *p = *(p + 1);
                ++p;
                *p = b;
                ++p;
                *(p - 3) = *p;
                *p = a;
            }
        }
        #endregion

        #region 64 Bits

        /// <summary>
        /// Swaps the endianness.
        /// </summary>
        /// <param name="array">The array.</param>
        public static void SwapEndianness(this long[] array)
        {
            unsafe
            {
                fixed (void* arr = array)
                    Swap64((byte*)arr, 0, array.Length);
            }
        }

        /// <summary>
        /// Swaps the endianness.
        /// </summary>
        /// <param name="array">The array.</param>
        public static void SwapEndianness(this ulong[] array)
        {
            unsafe
            {
                fixed (void* arr = array)
                    Swap64((byte*)arr, 0, array.Length);
            }
        }

        /// <summary>
        /// Swaps the endianness of a 64 bit integer at the specified byte offset.
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">How many integers to swap.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">offset</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">count</exception>
        public static void SwapInt64Endianness(this byte[] src, int offset = 0, int count = 1)
        {
            if (offset >= src.Length) throw new ArgumentOutOfRangeException("offset");
            if (offset + count * 8 >= src.Length) throw new ArgumentOutOfRangeException("count");
            unsafe
            {
                fixed (byte* arr = src)
                    Swap64(arr, offset, count);
            }
        }

        /// <summary>
        /// Swaps the endianness of 64 bit integers starting at the specified byte offset, processing <param name="count"/> full 64 bit integers (Bytes X 8).
        /// </summary>
        /// <param name="src">The SRC.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        public static unsafe void Swap64(byte* src, int offset, int count)
        {
            var p = src + offset;
            while (count-- > 0)
            {
                var a = *p;
                var b = *(p + 1);
                *p = *(p + 7);
                *(p + 7) = a;
                ++p;
                *p = *(p + 5);
                *(p + 5) = b;
                ++p;
                a = *p;
                b = *(p + 1);
                *p = *(p + 3);
                *(p + 3) = a;
                ++p;
                *p = *(p + 1);
                *(p + 1) = b;
            }
        }
        #endregion

        /// <summary>
        /// Builds the pack.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public static Uri MakePackUri(Assembly assembly, string file)
        {
            return new Uri(String.Format("pack://application:,,,/{0};component/{1}", assembly.GetName().Name, file));
        }

        /// <summary>
        /// Guards the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void Guard(this object obj, string name)
        {
            if (null == obj)
                throw new ArgumentNullException(name);
        }

        /// <summary>
        /// Guards the string.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void GuardString(this string str, string name)
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentNullException(name);
        }
    }
}