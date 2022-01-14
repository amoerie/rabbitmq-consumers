using EasyNetQ;
using Microsoft.Extensions.Logging;

namespace RabbitConsumers;

public class TestQueueItemConsumer
{
    private readonly ILogger<TestQueueItemConsumer> _logger;
    private int _isFirstItem;

    public TestQueueItemConsumer(ILogger<TestQueueItemConsumer> logger)
    {
        _logger = logger;
        _isFirstItem = 1;
    }

    public async Task OnMessageAsync(IMessage<TestQueueItem> message, MessageReceivedInfo messageInfo, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received message {@Message} with info {@MessageInfo}", message.Body, messageInfo);
        
        var testQueueItem = message.Body;
        _logger.LogInformation("Processing test queue item {@TestQueueItem}", testQueueItem);

        if (Interlocked.CompareExchange(ref _isFirstItem, 0, 1) == 1)
        {
            _logger.LogInformation("Since this is the first item, waiting 90 seconds so the channel crashes");
            await Task.Delay(TimeSpan.FromSeconds(90), cancellationToken);
        }
        else
        {
            _logger.LogInformation("Since this NOT the first item, there is no processing delay");
        }

        _logger.LogInformation("Processed test queue item {@TestQueueItem} successfully", testQueueItem);
    }
}