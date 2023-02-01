using System;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Newtonsoft.Json;
namespace generic_kafka_library
{
    internal sealed class KafkaDeserializer<T> : IDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (typeof(T) == typeof(Null))
                if (data.Length > 0)
                    throw new ArgumentException("The dara is null");
            return default;

            if (typeof(T) == typeof(Ignore))
                return default;

            var dataJson = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<T>(dataJson);


        }
    }
}