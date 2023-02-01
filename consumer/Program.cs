using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "pkc-l6wr6.europe-west2.gcp.confluent.cloud:9092",
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = "",
            SaslPassword = "",
            GroupId = "kafka-dotnet-doingz-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,

            EnableAutoCommit = true,
            StatisticsIntervalMs = 5000,
            SessionTimeoutMs = 6000,
            EnablePartitionEof = true,
            PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky
        };
        const string topicName = "email_topic";

        CancellationTokenSource cancellationToken = new();
        Console.CancelKeyPress += (_, x) =>
        {
            x.Cancel = true;
            cancellationToken.Cancel();
        };

        using (var consumer = new ConsumerBuilder<Ignore, EmailMessage>(consumerConfig)
        .SetValueDeserializer(new EmailMessage())
        .SetLogHandler((_, logHandler) => { System.Console.WriteLine(logHandler.Message); })
        .SetErrorHandler((_, errorHandler) => { System.Console.WriteLine(errorHandler.Reason); })
        .Build())
        {
            consumer.Subscribe(topicName);
            try
            {
                while (true)
                {
                    var result = consumer.Consume(cancellationToken.Token);
                    System.Console.WriteLine($"Consumes event from topic {topicName} with key {result.TopicPartitionOffset} and value {result.Message.Value}");

                }
            }
            catch (OperationCanceledException)
            {

            }
            finally
            {
                consumer.Close();
            }
        }

    }
}
class EmailMessage : ISerializer<EmailMessage>, IDeserializer<EmailMessage>
{
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public string? To { get; set; }

    public EmailMessage Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        var json = Encoding.UTF8.GetString(data);
        return JsonConvert.DeserializeObject<EmailMessage>(json);
    }

    public byte[] Serialize(EmailMessage data, SerializationContext context)
    {
        var json = JsonConvert.SerializeObject(data);
        return Encoding.UTF8.GetBytes(json);
    }
}