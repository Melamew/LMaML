using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace iLynx.Common.Serialization
{
    /// <summary>
    /// ISerializerService
    /// </summary>
    public interface ISerializerService
    {
        /// <summary>
        /// Deserializes the specified source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        T Deserialize<T>(Stream source) where T : new();

        /// <summary>
        /// Serializes the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <param name="target">The target.</param>
        void Serialize<T>(T item, Stream target) where T : new();
    }
    /// <summary>
    /// Serializer
    /// </summary>
    public class Serializer : ComponentBase, ISerializerService
    {
        public static readonly Guid SerializerNamespace = new Guid("6CB4F946-4563-4458-938A-56E5DAB8640F");

        private static Serializer empty;
        private static Serializer Empty { get { return empty ?? (empty = new Serializer(new ConsoleLogger())); } }

        private static readonly Dictionary<Type, ITypeSerializer> LookupTable = new Dictionary<Type, ITypeSerializer>
                                                                                    {
                                                                                             { typeof(int), new Primitives.Int32Serializer() },
                                                                                             { typeof(uint), new Primitives.UInt32Serializer() },
                                                                                             { typeof(short), new Primitives.Int16Serializer() },
                                                                                             { typeof(ushort), new Primitives.UInt16Serializer() },
                                                                                             { typeof(long), new Primitives.Int64Serializer() },
                                                                                             { typeof(ulong), new Primitives.UInt64Serializer() },
                                                                                             { typeof(double), new Primitives.DoubleSerializer() },
                                                                                             { typeof(float), new Primitives.SingleSerializer() },
                                                                                             { typeof(decimal), new Primitives.DecimalSerializer() },
                                                                                             { typeof(byte), new Primitives.ByteSerializer() },
                                                                                             { typeof(sbyte), new Primitives.ByteSerializer() },
                                                                                             { typeof(char), new Primitives.CharSerializer() },
                                                                                             { typeof(string), new Primitives.StringSerializer() },
                                                                                             { typeof(Guid), new Primitives.GuidSerializer() },
                                                                                             { typeof(bool), new Primitives.BooleanSerializer() }
                                                                                         };

        private static readonly Dictionary<Type, object> ObjectSerializers = new Dictionary<Type, object>();
        private static readonly Dictionary<Type, object> OverrideSerializers = new Dictionary<Type, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Serializer" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public Serializer(ILogger logger)
            : base(logger)
        {
        }

        /// <summary>
        /// Overrides the serializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="makeSerializer">The make.</param>
        public static void OverrideSerializer<T>(Func<ILogger, ISerializer<T>> makeSerializer)
        {
            lock (OverrideSerializers)
            {
                if (!OverrideSerializers.ContainsKey(typeof(T)))
                    OverrideSerializers.Add(typeof(T), makeSerializer);
            }
        }

        /// <summary>
        /// The singleton bit converter
        /// </summary>
        public static readonly IBitConverter SingletonBitConverter = new BigEndianBitConverter();

        public static void AddTypeSerializer<T>(ITypeSerializer serializer)
        {
            lock (LookupTable)
            {
                if (!LookupTable.ContainsKey(typeof(T)))
                    LookupTable.Add(typeof(T), serializer);
            }
        }

        /// <summary>
        /// Gets the serializer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ITypeSerializer GetTypeSerializer<T>()
        {
            return GetTypeSerializer(typeof(T));
        }

        /// <summary>
        /// Gets the type serializer.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static ITypeSerializer GetTypeSerializer(Type type)
        {
            ITypeSerializer ser;
            lock (LookupTable)
            {
                if (!LookupTable.TryGetValue(type, out ser))
                {
                    ser = type.IsArray ? MakeArraySerializer(type) : MakeSerializer(type);
                    LookupTable.Add(type, ser);
                }
            }
            return ser;
        }

        private static ITypeSerializer MakeArraySerializer(Type arrayType)
        {
            return new Primitives.ArraySerializer(arrayType);
        }

        private static ITypeSerializer MakeSerializer(Type oType)
        {
            var serializeInfo = typeof(Serializer).GetMethod("Serialize", BindingFlags.Instance | BindingFlags.Public); // TODO: Make this a not-so-magic-string
            serializeInfo = serializeInfo.MakeGenericMethod(oType);
            var deserializeInfo = typeof(Serializer).GetMethod("Deserialize", BindingFlags.Instance | BindingFlags.Public); // TODO: Make this a not-so-magic-string
            deserializeInfo = deserializeInfo.MakeGenericMethod(oType);
            return new Primitives.CallbackSerializer((o, stream) => serializeInfo.Invoke(Empty, new[] { o, stream }),
                                            stream => deserializeInfo.Invoke(Empty, new object[] { stream }));
        }

        public ISerializer<T> GetSerializer<T>() where T : new()
        {
            object serializer;
            var overridden = false;
            if (!ObjectSerializers.TryGetValue(typeof(T), out serializer) &&
                !(overridden = OverrideSerializers.TryGetValue(typeof(T), out serializer)))
            {
                serializer = TryInstantiate<T>();
                if (null == serializer) return null; // Giving up at this point
                ObjectSerializers.Add(typeof(T), serializer);
            }
            if (overridden)
            {
                try
                {
                    serializer = ((Func<ILogger, ISerializer<T>>)serializer)(Logger);
                    ObjectSerializers.Add(typeof(T), serializer);
                }
                catch (Exception e)
                {
                    LogException(e, MethodBase.GetCurrentMethod());
                }
            }
            return serializer as ISerializer<T>;
        }

        private ISerializer<T> TryInstantiate<T>() where T : new()
        {
            try
            {
                return new NaiveSerializer<T>(Logger);
            }
            catch (Exception e)
            {
                LogException(e, MethodBase.GetCurrentMethod());
                return null;
            }
        }

        /// <summary>
        /// Serializes the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <param name="target">The target.</param>
        public void Serialize<T>(T item, Stream target) where T : new()
        {
            var serializer = GetSerializer<T>();
            if (null == serializer)
            {
                OnGetSerializerError<T>();
                return;
            }
            serializer.Serialize(item, target);
        }

        private void OnGetSerializerError<T>()
        {
            LogError("Something went wrong here... Tried to get Serializer for Type: {0}, but was unsuccesful", typeof(T));
        }

        /// <summary>
        /// Deserializes the specified source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public T Deserialize<T>(Stream source) where T : new()
        {
            var serializer = GetSerializer<T>();
            if (null == serializer)
            {
                OnGetSerializerError<T>();
                return default(T);
            }
            return serializer.Deserialize(source);
        }
    }

    public class Primitives
    {
        public class StringSerializer : ITypeSerializer
        {
            private Encoding encoding = Encoding.ASCII;
            public Encoding Encoding { get { return encoding; } set { encoding = value; } }
            public void WriteTo(object item, Stream target)
            {
                var str = item as string;
                if (null == str) return;
                var bytes = encoding.GetBytes(str);
                var len = bytes.Length;
                target.Write(Serializer.SingletonBitConverter.GetBytes(len), 0, sizeof(int));
                target.Write(bytes, 0, bytes.Length);
            }

            public object ReadFrom(Stream source)
            {
                var buffer = new byte[4];
                source.Read(buffer, 0, buffer.Length);
                var len = Serializer.SingletonBitConverter.ToInt32(buffer);
                buffer = new byte[len];
                source.Read(buffer, 0, buffer.Length);
                return encoding.GetString(buffer);
            }
        }
        //public class IntegerArraySerializer : ITypeSerializer
        //{
        //    private readonly int integerSize;

        //    /// <summary>
        //    /// Initializes a new instance of the <see cref="IntegerArraySerializer" /> class.
        //    /// </summary>
        //    /// <param name="integerSize">Size of the integer (In bytes).</param>
        //    /// <exception cref="System.NotSupportedException">Serializer only supports</exception>
        //    public IntegerArraySerializer(int integerSize = 4)
        //    {
        //        this.integerSize = integerSize;
        //        if (integerSize != 2 && integerSize != 4 && integerSize != 8)
        //            throw new NotSupportedException("Serializer currently only supports 16, 32 and 64 bit integers!");
        //    }

        //    public void WriteTo(object item, Stream target)
        //    {

        //    }

        //    private void Write16(short[] array, Stream target)
        //    {
        //        if (Serializer.SingletonBitConverter.Endianness != Endianness.Big)
        //            array.SwapEndianness();
        //        var buffer = new byte[array.Length*2];
        //    }

        //    public object ReadFrom(Stream source)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        /// <summary>
        /// GuidSerializer
        /// </summary>
        public class GuidSerializer : ITypeSerializer
        {
            /// <summary>
            /// Serializes the specified item.
            /// </summary>
            /// <param name="item">The item.</param>
            /// <param name="target">The target.</param>
            public void WriteTo(object item, Stream target)
            {
                if (!(item is Guid)) return;
                var guid = (Guid)item;
                var buf = guid.ToByteArray();
                target.Write(buf, 0, buf.Length);
            }

            /// <summary>
            /// Deserializes the specified source.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <returns></returns>
            public object ReadFrom(Stream source)
            {
                var buf = new byte[16];
                source.Read(buf, 0, 16);
                return new Guid(buf);
            }
        }

        /// <summary>
        /// CallbackSerializer
        /// </summary>
        public class CallbackSerializer : ITypeSerializer
        {
            private readonly Action<object, Stream> writeCallback;
            private readonly Func<Stream, object> readCallback;

            /// <summary>
            /// Initializes a new instance of the <see cref="iLynx.Common.Serialization.Primitives.CallbackSerializer" /> class.
            /// </summary>
            /// <param name="writeCallback">The write callback.</param>
            /// <param name="readCallback">The read callback.</param>
            public CallbackSerializer(Action<object, Stream> writeCallback, Func<Stream, object> readCallback)
            {
                writeCallback.Guard("writeCallback");
                readCallback.Guard("readCallback");
                this.writeCallback = writeCallback;
                this.readCallback = readCallback;
            }

            /// <summary>
            /// Serializes the specified item.
            /// </summary>
            /// <param name="item">The item.</param>
            /// <param name="target">The target.</param>
            public void WriteTo(object item, Stream target)
            {
                writeCallback(item, target);
            }

            /// <summary>
            /// Deserializes the specified source.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <returns></returns>
            public object ReadFrom(Stream source)
            {
                return readCallback(source);
            }
        }

        /// <summary>
        /// ArraySerializer
        /// // TODO: Maybe this could handle polymorphic arrays too?
        /// </summary>
        public class ArraySerializer : ITypeSerializer
        {
            private readonly Type elementType;
            private readonly ITypeSerializer itemSerializer;

            /// <summary>
            /// Initializes a new instance of the <see cref="iLynx.Common.Serialization.Primitives.ArraySerializer" /> class.
            /// </summary>
            /// <param name="arrayType">Type of the array.</param>
            /// <exception cref="WhatTheFuckException"></exception>
            public ArraySerializer(Type arrayType)
            {
                if (!arrayType.IsArray) throw new WhatTheFuckException();
                elementType = arrayType.GetElementType();
                itemSerializer = Serializer.GetTypeSerializer(elementType);
                if (null == itemSerializer) throw new WhatTheFuckException();
            }

            /// <summary>
            /// Writes to.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="target">The target.</param>
            /// <exception cref="WhatTheFuckException"></exception>
            public void WriteTo(object value, Stream target)
            {
                var array = value as Array;
                if (null == array) throw new WhatTheFuckException();

                using (var memStream = new MemoryStream())
                {
                    var cnt = array.Length;
                    for (var i = 0; i < cnt; ++i)
                        itemSerializer.WriteTo(array.GetValue(i), memStream);
                    var buffer = Serializer.SingletonBitConverter.GetBytes(cnt);
                    target.Write(buffer, 0, buffer.Length);
                    memStream.WriteTo(target);
                }
            }

            /// <summary>
            /// Deserializes the specified source.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <returns></returns>
            /// <exception cref="WhatTheFuckException"></exception>
            public object ReadFrom(Stream source)
            {
                var buffer = new byte[4];
                var count = source.Read(buffer, 0, buffer.Length);
                if (4 != count) throw new WhatTheFuckException();
                var elements = Serializer.SingletonBitConverter.ToInt32(buffer);
                var array = Array.CreateInstance(elementType, elements);
                for (var i = 0; i < elements; ++i)
                    array.SetValue(itemSerializer.ReadFrom(source), i);
                return array;
            }
        }

        /// <summary>
        /// BooleanSerializer
        /// </summary>
        public class BooleanSerializer : ITypeSerializer
        {
            /// <summary>
            /// Serializes the specified item.
            /// </summary>
            /// <param name="item">The item.</param>
            /// <param name="target">The target.</param>
            public void WriteTo(object item, Stream target)
            {
                if (!(item is bool)) return;
                var b = (bool)item;
                target.Write(new[] { (byte)(b ? 0x01 : 0x00) }, 0, 1);
            }

            /// <summary>
            /// Deserializes the specified source.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <returns></returns>
            public object ReadFrom(Stream source)
            {
                var buf = new byte[1];
                source.Read(buf, 0, 1);
                return buf[0] == 1;
            }
        }

        public class Int16Serializer : ITypeSerializer
        {
            /// <summary>
            /// Writes to.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="target">The target.</param>
            public void WriteTo(object value, Stream target)
            {
                target.Write(Serializer.SingletonBitConverter.GetBytes((short)value), 0, sizeof(short));
            }

            /// <summary>
            /// Reads from.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <returns></returns>
            public object ReadFrom(Stream source)
            {
                var buffer = new byte[sizeof(short)];
                source.Read(buffer, 0, buffer.Length);
                return Serializer.SingletonBitConverter.ToInt16(buffer);
            }
        }

        public class UInt16Serializer : ITypeSerializer
        {
            /// <summary>
            /// Writes to.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="target">The target.</param>
            public void WriteTo(object value, Stream target)
            {
                target.Write(Serializer.SingletonBitConverter.GetBytes((ushort)value), 0, sizeof(ushort));
            }

            /// <summary>
            /// Reads from.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <returns></returns>
            public object ReadFrom(Stream source)
            {
                var buffer = new byte[sizeof(ushort)];
                source.Read(buffer, 0, buffer.Length);
                return Serializer.SingletonBitConverter.ToUInt16(buffer);
            }
        }

        public class Int32Serializer : ITypeSerializer
        {
            /// <summary>
            /// Writes to.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="target">The target.</param>
            public void WriteTo(object value, Stream target)
            {
                target.Write(Serializer.SingletonBitConverter.GetBytes((int)value), 0, sizeof(int));
            }

            /// <summary>
            /// Reads from.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <returns></returns>
            public object ReadFrom(Stream source)
            {
                var buffer = new byte[sizeof(int)];
                source.Read(buffer, 0, buffer.Length);
                return Serializer.SingletonBitConverter.ToInt32(buffer);
            }
        }

        public class UInt32Serializer : ITypeSerializer
        {
            /// <summary>
            /// Writes to.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="target">The target.</param>
            public void WriteTo(object value, Stream target)
            {
                target.Write(Serializer.SingletonBitConverter.GetBytes((uint)value), 0, sizeof(int));
            }

            /// <summary>
            /// Reads from.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <returns></returns>
            public object ReadFrom(Stream source)
            {
                var buffer = new byte[sizeof(int)];
                source.Read(buffer, 0, buffer.Length);
                return Serializer.SingletonBitConverter.ToUInt32(buffer);
            }
        }

        public class Int64Serializer : ITypeSerializer
        {
            /// <summary>
            /// Writes to.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="target">The target.</param>
            public void WriteTo(object value, Stream target)
            {
                target.Write(Serializer.SingletonBitConverter.GetBytes((long)value), 0, sizeof(long));
            }

            /// <summary>
            /// Reads from.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <returns></returns>
            public object ReadFrom(Stream source)
            {
                var buffer = new byte[sizeof(long)];
                source.Read(buffer, 0, buffer.Length);
                return Serializer.SingletonBitConverter.ToInt64(buffer);
            }
        }

        public class UInt64Serializer : ITypeSerializer
        {
            /// <summary>
            /// Writes to.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="target">The target.</param>
            public void WriteTo(object value, Stream target)
            {
                target.Write(Serializer.SingletonBitConverter.GetBytes((ulong)value), 0, sizeof(ulong));
            }

            /// <summary>
            /// Reads from.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <returns></returns>
            public object ReadFrom(Stream source)
            {
                var buffer = new byte[sizeof(ulong)];
                source.Read(buffer, 0, buffer.Length);
                return Serializer.SingletonBitConverter.ToUInt64(buffer);
            }
        }

        public class DoubleSerializer : ITypeSerializer
        {
            /// <summary>
            /// Writes to.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="target">The target.</param>
            public void WriteTo(object value, Stream target)
            {
                target.Write(Serializer.SingletonBitConverter.GetBytes((double)value), 0, sizeof(double));
            }

            /// <summary>
            /// Reads from.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <returns></returns>
            public object ReadFrom(Stream source)
            {
                var buffer = new byte[sizeof(double)];
                source.Read(buffer, 0, buffer.Length);
                return Serializer.SingletonBitConverter.ToDouble(buffer);
            }
        }

        public class SingleSerializer : ITypeSerializer
        {
            /// <summary>
            /// Writes to.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="target">The target.</param>
            public void WriteTo(object value, Stream target)
            {
                target.Write(Serializer.SingletonBitConverter.GetBytes((float)value), 0, sizeof(float));
            }

            /// <summary>
            /// Reads from.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <returns></returns>
            public object ReadFrom(Stream source)
            {
                var buffer = new byte[sizeof(float)];
                source.Read(buffer, 0, buffer.Length);
                return Serializer.SingletonBitConverter.ToDouble(buffer);
            }
        }

        public class DecimalSerializer : ITypeSerializer
        {
            /// <summary>
            /// Writes to.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="target">The target.</param>
            public void WriteTo(object value, Stream target)
            {
                target.Write(Serializer.SingletonBitConverter.GetBytes((decimal)value), 0, sizeof(decimal));
            }

            /// <summary>
            /// Reads from.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <returns></returns>
            public object ReadFrom(Stream source)
            {
                var buffer = new byte[sizeof(decimal)];
                source.Read(buffer, 0, buffer.Length);
                return Serializer.SingletonBitConverter.ToDecimal(buffer);
            }
        }

        public class ByteSerializer : ITypeSerializer
        {
            /// <summary>
            /// Writes to.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="target">The target.</param>
            public void WriteTo(object value, Stream target)
            {
                target.Write(new[] { (byte)value }, 0, sizeof(byte));
            }

            /// <summary>
            /// Reads from.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <returns></returns>
            public object ReadFrom(Stream source)
            {
                var buf = new byte[1];
                source.Read(buf, 0, sizeof(byte));
                return buf[0];
            }
        }

        /// <summary>
        /// CharSerializer
        /// </summary>
        public class CharSerializer : ITypeSerializer
        {
            /// <summary>
            /// Serializes the specified item.
            /// </summary>
            /// <param name="item">The item.</param>
            /// <param name="target">The target.</param>
            public void WriteTo(object item, Stream target)
            {
                var buffer = new byte[sizeof(char)];
                Buffer.BlockCopy(new[] { item }, 0, buffer, 0, sizeof(char));
                target.Write(buffer, 0, sizeof(char));
            }

            /// <summary>
            /// Deserializes the specified source.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <returns></returns>
            public object ReadFrom(Stream source)
            {
                var buffer = new byte[sizeof(char)];
                source.Read(buffer, 0, sizeof(char));
                var result = new char[1];
                Buffer.BlockCopy(buffer, 0, result, 0, sizeof(char));
                return result[0];
            }
        }
    }
}
