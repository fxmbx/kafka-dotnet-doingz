using Confluent.Kafka;
using generic_kafka_library.Consumer;
using generic_kafka_library.Interface;
using generic_kafka_library.Producer;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var clientConfig = new ClientConfig()
{
    BootstrapServers = configuration["Kafka:ClientConfigs:BootstrapServers"]
};

var producerConfig = new ProducerConfig(clientConfig);
var consumerConfig = new ConsumerConfig(clientConfig)
{
    GroupId = "pkc-l6wr6.europe-west2.gcp.confluent.cloud:9092",
    EnableAutoCommit = true,
    AutoOffsetReset = AutoOffsetReset.Earliest,
    StatisticsIntervalMs = 5000,
    SessionTimeoutMs = 6000
};

builder.Services.AddSingleton(producerConfig);
builder.Services.AddSingleton(consumerConfig);

builder.Services.AddSingleton(typeof(IKafkaProducer<,>), typeof(KafkaProducer<,>));
builder.Services.AddSingleton(typeof(IKafkaConsumer<,>), typeof(KafkaConsumer<,>));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
