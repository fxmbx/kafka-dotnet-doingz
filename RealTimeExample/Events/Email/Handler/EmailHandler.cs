using generic_kafka_library.Interface;
using generic_kafka_library.Messages;

namespace RealTimeExample.Events.Email.Handler
{
    public class EmailHandler : IKafkaHandler<string, EmailMessage>
    {
        private readonly IKafkaProducer<string, EmailMessage> _producer;

        public EmailHandler(IKafkaProducer<string, EmailMessage> producer)
        {
            _producer = producer;
        }
        public Task HandleAsync(string key, EmailMessage value)
        {
            //write code to send email
            Console.WriteLine($"To: {value?.To}\n Subject: {value?.Subject}");

            _producer.ProduceAsync("email_sent", value?.To, new EmailMessage { To = value?.To });

            return Task.CompletedTask;
        }
    }
}