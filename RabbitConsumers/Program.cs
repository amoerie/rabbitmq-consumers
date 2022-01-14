using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitConsumers;

var rabbitMqConnectionString = "host=localhost;persistentMessages=true;publisherConfirms=true;timeout=60;username=dobco;password=dobco";

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.SetMinimumLevel(LogLevel.Trace);
        logging.AddSimpleConsole(o =>
        {
            o.TimestampFormat = "[HH:mm:ss.fff] ";
        });
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<RabbitMqHostedService>();
        services.AddSingleton<TestQueueItemConsumer>();
        services.RegisterEasyNetQ(rabbitMqConnectionString, easyNetQServices =>
        {
            easyNetQServices.EnableNewtonsoftJson();
            easyNetQServices.EnableMicrosoftLogging();
        });
    })
    .UseConsoleLifetime()
    .Build();

await host.RunAsync();
