using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace iLynx.Common.Serialization
{
    /// <summary>
    /// NaiveSerializer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NaiveSerializer<T> : ComponentBase, ISerializer<T> where T : new()
    {
        public class SerializationInfo
        {
            public MemberInfo Member { get; private set; }
            public ITypeSerializer TypeSerializer { get; private set; }
            public bool IsUntyped { get; private set; }

            public SerializationInfo(MemberInfo member, ITypeSerializer serializer, bool isUntyped)
            {
                member.Guard("member");
                serializer.Guard("serializer");
                IsUntyped = isUntyped;
                Member = member;
                TypeSerializer = serializer;
            }

            public object GetValue(object source)
            {
                var property = Member as PropertyInfo;
                if (null != property)
                    return property.GetValue(source);
                var field = Member as FieldInfo;
                return null != field ? field.GetValue(source) : null;
            }

            public void SetValue(object target, object value)
            {
                var property = Member as PropertyInfo;
                if (null != property)
                {
                    property.SetValue(target, value);
                    return;
                }
                var field = Member as FieldInfo;
                if (null == field) return;
                field.SetValue(target, value);
            }
        }
        private readonly IEnumerable<SerializationInfo> sortedGraph;
        private const BindingFlags FieldFlags = BindingFlags.GetField | BindingFlags.SetField | BindingFlags.Public | BindingFlags.Instance;
        private const BindingFlags PropertyFlags = BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="NaiveSerializer{T}" /> class.
        /// </summary>
        /// <exception cref="System.NotSupportedException">Missing Types are currently not supported</exception>
        public NaiveSerializer(ILogger logger)
            : base(logger)
        {
            if (typeof(T) == Type.Missing.GetType()) throw new NotSupportedException("Missing Types are currently not supported");
            sortedGraph = BuildObjectGraph(typeof(T)).Values;
        }

        /// <summary>
        /// Builds the object graph.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns></returns>
        public static SortedList<Guid, SerializationInfo> BuildObjectGraph(Type targetType)
        {
            var graph = new SortedList<Guid, SerializationInfo>();
            var namespaceAttribute = targetType.GetCustomAttribute<GuidAttribute>();
            var name = Guid.Empty;
            var hasAttribute = false;
            if (null != namespaceAttribute) // Could be set to Guid.Empty for whatever reason...
            {
                name = new Guid(namespaceAttribute.Value);
                hasAttribute = true;
            }
            foreach (
                var fieldInfo in
                    targetType.GetFields(FieldFlags)
                              .Where(f => !f.IsDefined(typeof(NotSerializedAttribute)))
                              .Select(c => new SerializationInfo(c, GetSerializer(c.FieldType), c.FieldType == typeof(object)))
                              .Concat(targetType.GetProperties(PropertyFlags)    // Apparently BindingFLAGS don't work like flags...
                              .Where(p => null != p.SetMethod && p.SetMethod.IsPublic && null != p.GetMethod && p.GetMethod.IsPublic)
                              .Where(p => !p.IsDefined(typeof(NotSerializedAttribute)))
                              .Select(p =>new SerializationInfo(p, GetSerializer(p.PropertyType), p.PropertyType == typeof(object))))
                )
            {
                var id = hasAttribute
                             ? fieldInfo.Member.Name.CreateGuidV5(name)
                             : fieldInfo.Member.Name.CreateGuidV5(Serializer.SerializerNamespace);
                graph.Add(id, fieldInfo);
            }
            return graph;
        }

        private static ITypeSerializer GetSerializer(Type type)
        {
            return Serializer.GetTypeSerializer(type.IsEnum ? Enum.GetUnderlyingType(type) : type);
        }

        /// <summary>
        /// Deserializes the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public T Deserialize(Stream source)
        {
            var target = new T();
            foreach (var member in sortedGraph)
            {
                ITypeSerializer serializer;
                if (member.IsUntyped)
                {
                    var memberType = ReadType(source);
                    if (null == memberType) continue;
                    serializer = Serializer.GetTypeSerializer(memberType);
                }
                else serializer = member.TypeSerializer;

                try
                {
                    var value = serializer.ReadFrom(source);
                    member.SetValue(target, value);
                }
                catch (Exception e)
                {
                    PostQuit(e, MethodBase.GetCurrentMethod());
                    break;
                }
            }
            return target;
        }

        /// <summary>
        /// Posts the quit.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="m">The m.</param>
        private void PostQuit(Exception e, MethodBase m)
        {
            LogException(e, m);
            LogCritical("Last Error was unrecoverable. Giving up");
        }

        // ReSharper disable StaticFieldInGenericType
        private static readonly Encoding Unicode = Encoding.Unicode;
        // ReSharper restore StaticFieldInGenericType

        /// <summary>
        /// Serializes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="target">The target.</param>
        public void Serialize(T item, Stream target)
        {
            foreach (var member in sortedGraph)
            {
                var value = member.GetValue(item);
                ITypeSerializer serializer;
                if (member.IsUntyped)
                {
                    serializer = Serializer.GetTypeSerializer((value ?? new NullType()).GetType());
                    WriteType(target, value);
                }
                else serializer = member.TypeSerializer;
                try
                {
                    serializer.WriteTo(value, target);
                }
                catch (Exception e)
                {
                    PostQuit(e, MethodBase.GetCurrentMethod());
                    break;
                }
            }
        }

        private class NullType { }

        /// <summary>
        /// Reads the type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        private static Type ReadType(Stream source)
        {
            var length = new byte[sizeof(int)];
            source.Read(length, 0, length.Length);
            var len = Serializer.SingletonBitConverter.ToInt32(length);
            if (len <= 0 || len >= 4096)
                return null;
            var field = new byte[len];
            source.Read(field, 0, field.Length);
            var typeString = Unicode.GetString(field);
            return Type.GetType(typeString);//, name => Assembly.Load(name.FullName), (assembly, s, arg3) => assembly == null ? Type.GetType(s) : assembly.GetType(s, false, arg3));
        }

        /// <summary>
        /// Writes the type.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="o">The o.</param>
        private static void WriteType(Stream target, object o)
        {
            var type = o.GetType();
            var typeBytes = Unicode.GetBytes(type.AssemblyQualifiedName ?? type.FullName);
            var length = Serializer.SingletonBitConverter.GetBytes(typeBytes.Length);
            target.Write(length, 0, length.Length);
            target.Write(typeBytes, 0, typeBytes.Length);
        }
    }

    public class NotSerializedAttribute : Attribute
    {
    }

    public class WhatTheFuckException : Exception
    {

    }
}
