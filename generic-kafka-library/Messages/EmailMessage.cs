using System.ComponentModel.DataAnnotations;

namespace generic_kafka_library.Messages
{
    public class EmailMessage
    {
        [Required]
        public string? Subject { get; set; }
        [Required]

        public string? Body { get; set; }
        [Required]

        public string? To { get; set; }

    }
}