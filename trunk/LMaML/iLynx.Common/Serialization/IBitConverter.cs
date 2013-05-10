namespace iLynx.Common.Serialization
{
    /// <summary>
    /// IBitConverterService
    /// </summary>
    public interface IBitConverter
    {
        byte[] GetBytes(int value);
        byte[] GetBytes(uint value);
        byte[] GetBytes(short value);
        byte[] GetBytes(ushort value);
        byte[] GetBytes(long value);
        byte[] GetBytes(ulong value);
        byte[] GetBytes(float value);
        byte[] GetBytes(double value);
        byte[] GetBytes(decimal value);
        decimal ToDecimal(byte[] array, int startIndex = 0);
        int ToInt32(byte[] array, int startIndex = 0);
        uint ToUInt32(byte[] array, int startIndex = 0);
        short ToInt16(byte[] array, int startIndex = 0);
        ushort ToUInt16(byte[] array, int startIndex = 0);
        long ToInt64(byte[] array, int startIndex = 0);
        ulong ToUInt64(byte[] array, int startIndex = 0);
        float ToSingle(byte[] array, int startIndex = 0);
        double ToDouble(byte[] array, int startIndex = 0);
        /// <summary>
        /// Gets the endianness of this bitconverter (Assuming it is running on a Big Endian platform).
        /// </summary>
        /// <value>
        /// The endianness.
        /// </value>
        Endianness Endianness { get; }
    }
}
