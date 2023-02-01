namespace generic_kafka_library
{
    public class KafkaTopic
    {
        public string TopicName;
        public void SetTopicName(string topic = "email_topic")
        {
            TopicName = topic;
        }

    }
}