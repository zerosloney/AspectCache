using System;
using System.Text;
using MessagePack;
using MessagePack.Resolvers;

namespace Aspect.Repository.Cache
{
    public class MsgPackSerializer : ISerializer
    {
        private Encoding EncodingFormat { get; }

        public Encoding Encoding => EncodingFormat;

        static MsgPackSerializer()
        {
            CompositeResolver.RegisterAndSetAsDefault(
                NativeDateTimeResolver.Instance,
                StandardResolver.Instance);
        }

        public MsgPackSerializer()
        {
            EncodingFormat = Encoding.UTF8;
        }

        public byte[] Serialize(Type type, object obj)
        {
            return MessagePackSerializer.NonGeneric.Serialize(type, obj);
        }

        public byte[] Serialize<T>(T t)
        {
            return MessagePackSerializer.Serialize(t);
        }

        public object Deserialize(Type type, byte[] serializedObject)
        {
            return MessagePackSerializer.NonGeneric.Deserialize(type, serializedObject);
        }

        public T Deserialize<T>(byte[] serializedObject)
        {
            return MessagePackSerializer.Deserialize<T>(serializedObject);
        }
    }
}