using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitConsumers;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.AddConsole();
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<RabbitMqHostedService>();
        services.AddSingleton<TestQueueItemConsumer>();
    })
    .UseConsoleLifetime()
    .Build();

await host.RunAsync();
