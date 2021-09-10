using System.IO;

namespace Azure.ServiceBus.Handler.Application.ProtobufSerializer
{
    public class Serializer
    {
        public static T Deserialize<T>(byte[] data)
            => data == null
                ? default(T)
                : ProtoBuf.Serializer.Deserialize<T>(new MemoryStream(data));

        public static byte[] Serialize(object obj)
        {
            using var stream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(stream, obj);

            return stream.ToArray();
        }
    }
}
