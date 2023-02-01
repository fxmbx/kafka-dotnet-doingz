using generic_kafka_library.Interface;
using generic_kafka_library.Messages;
using Microsoft.AspNetCore.Mvc;

namespace RealTimeExample.Controllers;

[ApiController]
[Route("[controller]")]
public class EmailController : ControllerBase
{
    private readonly ILogger<EmailController> _logger;

    private readonly IKafkaProducer<string, EmailMessage> kafkaProducer;

    public EmailController(ILogger<EmailController> logger, IKafkaProducer<string, EmailMessage> _kafkaProducer)
    {
        _logger = logger;
        kafkaProducer = _kafkaProducer;
    }

    [HttpPost]
    [Route("SendEmail")]
    public async Task<IActionResult> ProduceMessage(EmailMessage request)
    {
        await kafkaProducer.ProduceAsync("email_topic", request.To!, request);

        return Ok("Email sending In Progress");
    }
}
