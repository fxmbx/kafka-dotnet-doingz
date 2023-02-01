using Confluent.Kafka;
using generic_kafka_library.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace generic_kafka_library.Consumer
{
    public class KafkaConsumer<TKey, TValue> : IKafkaConsumer<TKey, TValue> where TValue : class
    {
        private readonly ConsumerConfig consumerConfig;
        private IKafkaHandler<TKey, TValue> _kafkaHandler;

        private IConsumer<TKey, TValue> _consumer;

        private string topicName;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public KafkaConsumer(ConsumerConfig config, IServiceScopeFactory serviceScopeFactory)
        {
            consumerConfig = config;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Consume(string topic, CancellationToken cancellationToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            _kafkaHandler = scope.ServiceProvider.GetRequiredService<IKafkaHandler<TKey, TValue>>();
            _consumer = new ConsumerBuilder<TKey, TValue>(consumerConfig).SetValueDeserializer(new KafkaDeserializer<TValue>()).Build();
            topicName = topic;

            await Task.Run(() => StartConsumerLoop(cancellationToken), cancellationToken);
        }

        private async Task StartConsumerLoop(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(topicName);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(cancellationToken);

                    if (result != null)
                    {
                        await _kafkaHandler.HandleAsync(result.Message.Key, result.Message.Value);
                    }
                }
                catch (OperationCanceledException)
                {

                    break;
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Consume error: {e.Error.Reason}");

                    if (e.Error.IsFatal)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error: {e}");
                    break;
                }
            }
        }
        public void Close()
        {
            _consumer.Close();
        }
        public void Dispose()
        {
            _consumer.Dispose();
        }
    }
}