using System.Net;
using generic_kafka_library.Interface;
using generic_kafka_library.Messages;

namespace RealTimeExample.Events.Email.Consumers
{
    public class EmailConsumer : BackgroundService
    {
        private readonly IKafkaConsumer<string, EmailMessage> kafkaconsumer;

        public EmailConsumer(IKafkaConsumer<string, EmailMessage> _consumer)
        {
            kafkaconsumer = _consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                await kafkaconsumer.Consume("email_topic", cancellationToken);

            }
            catch (System.Exception ex)
            {
                await kafkaconsumer.Consume("email_topic", cancellationToken);
                Console.WriteLine($"{(int)HttpStatusCode.InternalServerError} ConsumeFailedOnTopic - email_topic, {ex}");
                throw;
            }
        }
        public override void Dispose()
        {
            kafkaconsumer.Close();
            kafkaconsumer.Dispose();

            base.Dispose();
        }

    }
}