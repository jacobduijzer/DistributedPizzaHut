using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PaymentMicroservice;

public class ConsoleHostedService : IHostedService
{
    private readonly ILogger _logger;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IBusControl _busControl;

    public ConsoleHostedService(
        ILogger<ConsoleHostedService> logger,
        IHostApplicationLifetime appLifetime,
        IBusControl busControl)
    {
        _logger = logger;
        _appLifetime = appLifetime;
        _busControl = busControl;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Starting with arguments: {string.Join(" ", Environment.GetCommandLineArgs())}");

        _appLifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(async () =>
            {
                try
                {
                    var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                    await _busControl.StartAsync(source.Token);
                    Console.WriteLine("Order Microservice Now Listening");
                    try
                    {
                        while (true)
                        {
                            //sit in while loop listening for messages
                            await Task.Delay(100);
                        }
                    }
                    finally
                    {
                        await _busControl.StopAsync();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception!");
                }
                finally
                {
                    // Stop the application once the work is done
                    _appLifetime.StopApplication();
                }
            });
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}