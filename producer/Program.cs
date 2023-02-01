// See https://aka.ms/new-console-template for more information
using System.Text.Json.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Newtonsoft.Json;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        const string topicName = "email_topic";

        EmailMessage[] emailMessages = {
            new EmailMessage{ Subject = "Email Test1", Body = "This is the body for the First email",To="funbiolaore@gmail.com"},
            new EmailMessage{ Subject = "Email Test2", Body = "This is the body for the Second email",To="fxmbxolxorx@gmail.com"},
            new EmailMessage{ Subject = "Email Test3", Body = "This is the body for the Third email",To="funbi@morerave.com"},
            };
        var producerConfiguratoin = new ProducerConfig
        {
            BootstrapServers = "pkc-l6wr6.europe-west2.gcp.confluent.cloud:9092",
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = "",
            SaslPassword = ""
        };

        using var producer = new ProducerBuilder<string, EmailMessage>(producerConfiguratoin)
        .SetValueSerializer(new EmailMessage())
        .Build();
        var numProduced = 0;
        Random rnd = new();
        const int numMessages = 10;
        for (int i = 0; i < numMessages; ++i)
        {
            var emailMessage = emailMessages[rnd.Next(emailMessages.Length)];

            producer.Produce(topicName, new Message<string, EmailMessage> { Key = emailMessage.To!, Value = emailMessage },

            (deliveryReport) =>
            {
                if (deliveryReport.Error.Code != ErrorCode.NoError)
                {
                    Console.WriteLine($"Failed to deliver Message : {deliveryReport.Error.Reason}");
                }
                else
                {
                    Console.WriteLine($"Prodiced event to topic {topicName}: key = {emailMessage.To,-10} value = {emailMessage}");
                    numProduced += 1;
                }
            });
        }

        producer.Flush(TimeSpan.FromSeconds(10));
        Console.WriteLine($"{numProduced} messages were produced to topic {topicName}");
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