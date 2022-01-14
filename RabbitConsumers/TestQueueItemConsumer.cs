using Microsoft.Extensions.Logging;

namespace RabbitConsumers;

public class TestQueueItemConsumer
{
    private readonly ILogger<TestQueueItemConsumer> _logger;

    public TestQueueItemConsumer(ILogger<TestQueueItemConsumer> logger)
    {
        _logger = logger;
    }

    public async Task OnTestQueueItemAsync(TestQueueItem testQueueItem, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing test queue item {@TestQueueItem}", testQueueItem);

        await Task.Delay(1000, cancellationToken);
        
        _logger.LogInformation("Processed test queue item {@TestQueueItem} successfully", testQueueItem);
    }
}