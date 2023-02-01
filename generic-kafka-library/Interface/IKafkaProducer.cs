using System.Threading.Tasks;

namespace generic_kafka_library.Interface;

public interface IKafkaProducer<in TKey, in TValue> where TValue : class
{
    Task ProduceAsync(string topic, TKey key, TValue value);
}
