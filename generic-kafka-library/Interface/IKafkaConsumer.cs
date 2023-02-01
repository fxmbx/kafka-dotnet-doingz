using System.Threading;
using System.Threading.Tasks;

namespace generic_kafka_library.Interface;


public interface IKafkaConsumer<TKey, TValue> where TValue : class
{
    Task Consume(string topic, CancellationToken stoppingToken);
    void Close();
    void Dispose();
}
