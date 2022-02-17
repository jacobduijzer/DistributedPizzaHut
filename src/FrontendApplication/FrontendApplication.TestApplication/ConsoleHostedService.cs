using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FrontendApplication.TestApplication;

internal class ConsoleHostedService : IHostedService
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
                    _logger.LogInformation("Hello World!");
                    
                    await Task.Delay(3000); //because the consumers need to start first
                    
                    await _busControl.StartAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);
                    var command = new CreateOrderCommand(_busControl);
                    var keyCount = 0;
                    try
                    {
                        Console.WriteLine("Enter any key to send an invoice request or Q to quit.");
                        while (Console.ReadKey(true).Key != ConsoleKey.Q)
                        {
                            keyCount++;
                            // await SendRequestForInvoiceCreation(busControl);
                            await command.Handle();
                            Console.WriteLine($"Enter any key to send an invoice request or Q to quit. {keyCount}");
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