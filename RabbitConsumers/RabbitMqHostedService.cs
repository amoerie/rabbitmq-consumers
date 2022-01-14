using EasyNetQ;
using EasyNetQ.Topology;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ExchangeType = RabbitMQ.Client.ExchangeType;

namespace RabbitConsumers;

public class RabbitMqHostedService : BackgroundService
{
    private readonly ILogger<RabbitMqHostedService> _logger;
    private readonly IBus _rabbitMqBus;
    private readonly TestQueueItemConsumer _rabbitMqConsumer;

    public RabbitMqHostedService(ILogger<RabbitMqHostedService> logger,
        IBus rabbitMqBus,
        TestQueueItemConsumer rabbitMqConsumer)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _rabbitMqBus = rabbitMqBus ?? throw new ArgumentNullException(nameof(rabbitMqBus));
        _rabbitMqConsumer = rabbitMqConsumer ?? throw new ArgumentNullException(nameof(rabbitMqConsumer));
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Configuring exchange & queue");

        var testItemQueue  = await _rabbitMqBus.Advanced.QueueDeclareAsync("TestQueueItemsQueue", cancellationToken);
        var testItemExchange = await _rabbitMqBus.Advanced.ExchangeDeclareAsync("TestQueueItemsExchange", ExchangeType.Direct, true, false, cancellationToken);
        await _rabbitMqBus.Advanced.BindAsync(testItemExchange, testItemQueue, "#", cancellationToken);
        
        _logger.LogInformation("Configuration succeeded");

        _rabbitMqBus.Advanced.Consume<TestQueueItem>(
            testItemQueue,
            (message, messageInfo, token) => _rabbitMqConsumer.OnMessageAsync(message, messageInfo, token),
            config =>
            {
                config.WithPrefetchCount(1);
                config.WithConsumerTag("TestQueueItemConsumer");
            }
        );

        _logger.LogInformation("Consumer setup successfully");

        _logger.LogInformation("Publishing test queue items");

        await _rabbitMqBus.Advanced.PublishAsync(testItemExchange, "#", true, new Message<TestQueueItem>(new TestQueueItem("Test1")), cancellationToken);
        
        await _rabbitMqBus.Advanced.PublishAsync(testItemExchange, "#", true, new Message<TestQueueItem>(new TestQueueItem("Test2")), cancellationToken);

        _logger.LogInformation("Test queue items published successfully");
    }
}