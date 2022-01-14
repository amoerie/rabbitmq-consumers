using EasyNetQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace RabbitConsumers;

public class RabbitMqHostedService : BackgroundService
{
    private readonly ILogger<RabbitMqHostedService> _logger;
    private readonly TestQueueItemConsumer _consumer;

    public RabbitMqHostedService(ILogger<RabbitMqHostedService> logger, TestQueueItemConsumer consumer)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Opening connection to Rabbit MQ");

        var connectionString = "host=localhost;persistentMessages=true;publisherConfirms=true;timeout=60;username=dobco;password=dobco";
        
        var bus = RabbitHutch.CreateBus(connectionString, services =>
        {
            services.EnableConsoleLogger();
            services.EnableNewtonsoftJson();
        });
        
        _logger.LogInformation("Connection (seems to be) successful");

        _logger.LogInformation("Setting up consumer");

        bus.PubSub.Subscribe<TestQueueItem>(
            "TestQueueItemSubscriber", 
            (item, token) => _consumer.OnTestQueueItemAsync(item, token),
            config =>
            {
            },
            cancellationToken
        );
        
        _logger.LogInformation("Consumer setup successfully");
        
        _logger.LogInformation("Publishing test queue item");

        await bus.PubSub.PublishAsync(new TestQueueItem("Test1"), cancellationToken);
        
        _logger.LogInformation("Test queue item published successfully");
    }
}