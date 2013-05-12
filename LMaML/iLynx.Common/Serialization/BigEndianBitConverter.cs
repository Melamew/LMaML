using System;
using System.Diagnostics;

namespace iLynx.Common.Serialization
{
    /// <summary>
    /// MaskingBigEndianBitConverter
    /// </summary>
    public class BigEndianBitConverter : IBitConverter
    {
        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public byte[] GetBytes(int value)
        {
            return unchecked(new[]
                {
                    (byte) ((value >> 24) & 0xFF),
                    (byte) ((value >> 16) & 0xFF),
                    (byte) ((value >> 8) & 0xFF),
                    (byte) ((value) & 0xFF)
                });
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public byte[] GetBytes(uint value)
        {
            return unchecked(new[]
                {
                    (byte) ((value >> 24) & 0xFF),
                    (byte) ((value >> 16) & 0xFF),
                    (byte) ((value >> 8) & 0xFF),
                    (byte) ((value) & 0xFF)
                });
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public byte[] GetBytes(short value)
        {
            return unchecked(new[]
                {
                    (byte) ((value >> 8) & 0xFF),
                    (byte) (value & 0xFF)
                });
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public byte[] GetBytes(ushort value)
        {
            return unchecked(new[]
                {
                    (byte) ((value >> 8) & 0xFF),
                    (byte) (value & 0xFF)
                });
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public byte[] GetBytes(long value)
        {
            return unchecked(new[]
                {
                    (byte) ((value >> 56) & 0xFF),
                    (byte) ((value >> 48) & 0xFF),
                    (byte) ((value >> 40) & 0xFF),
                    (byte) ((value >> 32) & 0xFF),
                    (byte) ((value >> 24) & 0xFF),
                    (byte) ((value >> 16) & 0xFF),
                    (byte) ((value >> 8) & 0xFF),
                    (byte) (value & 0xFF)
                });
        }

        public byte[] GetBytes(ulong value)
        {
            return unchecked(new[]
                {
                    (byte) ((value >> 56) & 0xFF),
                    (byte) ((value >> 48) & 0xFF),
                    (byte) ((value >> 40) & 0xFF),
                    (byte) ((value >> 32) & 0xFF),
                    (byte) ((value >> 24) & 0xFF),
                    (byte) ((value >> 16) & 0xFF),
                    (byte) ((value >> 8) & 0xFF),
                    (byte) (value & 0xFF)
                });
        }

        public byte[] GetBytes(float value)
        {
            var buf = new byte[sizeof(float)];
            Buffer.BlockCopy(new[] { value }, 0, buf, 0, sizeof(float));
            return buf;
        }

        public byte[] GetBytes(double value)
        {
            var buf = new byte[sizeof(double)];
            Buffer.BlockCopy(new[] { value }, 0, buf, 0, sizeof(double));
            return buf;
        }

        public byte[] GetBytes(decimal value)
        {
            var buf = new byte[sizeof(decimal)];
            Buffer.BlockCopy(new[] { value }, 0, buf, 0, sizeof(decimal));
            return buf;
        }

        public int ToInt32(byte[] array, int startIndex = 0)
        {
            return unchecked(
                (array[startIndex] << 24) |
                (array[startIndex + 1] << 16) |
                (array[startIndex + 2] << 8) |
                (array[startIndex + 3]));
        }

        public uint ToUInt32(byte[] array, int startIndex = 0)
        {
            return unchecked(
                (uint)(
                (array[startIndex] << 24) |
                (array[startIndex + 1] << 16) |
                (array[startIndex + 2] << 8) |
                (array[startIndex + 3])
                ));
        }

        public short ToInt16(byte[] array, int startIndex = 0)
        {
            return unchecked(
                (short)(
                (array[startIndex] << 8) |
                (array[startIndex + 1])
                ));
        }

        public ushort ToUInt16(byte[] array, int startIndex = 0)
        {
            return unchecked(
                (ushort)(
                (array[startIndex] << 8) |
                (array[startIndex + 1])
                ));
        }

        public long ToInt64(byte[] array, int startIndex = 0)
        {
            long value = 0;
            unchecked
            {
                value |= array[startIndex];
                value <<= 8;
                value |= array[startIndex + 1];
                value <<= 8;
                value |= array[startIndex + 2];
                value <<= 8;
                value |= array[startIndex + 3];
                value <<= 8;
                value |= array[startIndex + 4];
                value <<= 8;
                value |= array[startIndex + 5];
                value <<= 8;
                value |= array[startIndex + 6];
                value <<= 8;
                value |= array[startIndex + 7];
            }
            return value;
            //return unchecked (((array[startIndex + 7]) |
            //                   (array[startIndex + 6] << 8) |
            //                   (array[startIndex + 5] << 16) |
            //                   (array[startIndex + 4] << 24) |
            //                   (array[startIndex + 3] << 32) |
            //                   (array[startIndex + 2] << 40) |
            //                   (array[startIndex + 1] << 48) |
            //                   (array[startIndex] << 56)));
        }

        public ulong ToUInt64(byte[] array, int startIndex = 0)
        {
            ulong value = 0;
            //ulong other;
            unchecked
            {
                value |= array[startIndex];
                value <<= 8;
                value |= array[startIndex + 1];
                value <<= 8;
                value |= array[startIndex + 2];
                value <<= 8;
                value |= array[startIndex + 3];
                value <<= 8;
                value |= array[startIndex + 4];
                value <<= 8;
                value |= array[startIndex + 5];
                value <<= 8;
                value |= array[startIndex + 6];
                value <<= 8;
                value |= array[startIndex + 7];
                //other = ((ulong)(
                //(array[startIndex + 7]) |
                //(array[startIndex + 6] << 8) |
                //(array[startIndex + 5] << 16) |
                //(array[startIndex + 4] << 24) |
                //(array[startIndex + 3] << 32) |
                //(array[startIndex + 2] << 40) |
                //(array[startIndex + 1] << 48) |
                //(array[startIndex] << 56)
                //));
            }
            //Debug.Assert(other == value);
            return value;
            //return unchecked ((ulong)(
            //    (array[startIndex]) |
            //    (array[startIndex + 1] << 8) |
            //    (array[startIndex + 2] << 16) |
            //    (array[startIndex + 3] << 24) |
            //    (array[startIndex + 4] << 32) |
            //    (array[startIndex + 5] << 40) |
            //    (array[startIndex + 6] << 48) |
            //    (array[startIndex + 7] << 56)
            //    ));
        }

        public float ToSingle(byte[] array, int startIndex = 0)
        {
            var buf = new float[1];
            Buffer.BlockCopy(array, startIndex, buf, 0, sizeof(float));
            return buf[0];
        }

        public double ToDouble(byte[] array, int startIndex = 0)
        {
            var buf = new double[1];
            Buffer.BlockCopy(array, startIndex, buf, 0, sizeof(double));
            return buf[0];
        }

        public decimal ToDecimal(byte[] array, int startIndex = 0)
        {
            var buf = new decimal[1];
            Buffer.BlockCopy(array, startIndex, buf, 0, sizeof(decimal));
            return buf[0];
        }

        /// <summary>
        /// Gets the endianness of this bitconverter (Assuming it is running on a Big Endian platform).
        /// </summary>
        /// <value>
        /// The endianness.
        /// </value>
        public Endianness Endianness { get { return Endianness.Big; } }
    }
}
