using FrontendApplication.TestApplication;
using GreenPipes;
using MassTransit;
using MessageContracts;

Console.WriteLine("Waiting while consumers initialize.");
await Task.Delay(3000); //because the consumers need to start first
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("localhost");
    cfg.ReceiveEndpoint("invoice-service-created", e =>
    {
        e.UseInMemoryOutbox();
        e.Consumer<PaymentToMakeEventConsumer>(c =>
            c.UseMessageRetry(m => m.Interval(5, new TimeSpan(0, 0, 10))));
    });
});

var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
await busControl.StartAsync(source.Token);
var keyCount = 0;
try
{
    Console.WriteLine("Enter any key to send an invoice request or Q to quit.");
    while (Console.ReadKey(true).Key != ConsoleKey.Q)
    {
        keyCount++;
        await SendRequestForInvoiceCreation(busControl);
        Console.WriteLine($"Enter any key to send an invoice request or Q to quit. {keyCount}");
    }
}
finally
{
    await busControl.StopAsync();
}

static async Task SendRequestForInvoiceCreation(IPublishEndpoint publishEndpoint)
{
    await publishEndpoint.Publish<IOrderToCreate>(new
    {
        CustomerNumber = Guid.NewGuid(),
        Pizzas = new List<Pizza>()
        {
            new Pizza
            {
                PizzaId = Guid.NewGuid(),
                Name = "Some Pizza",
                Price = 5.95
            },
            new Pizza
            {
                PizzaId = Guid.NewGuid(),
                Name = "Another Pizza",
                Price = 6.95
            }
        }
    });
}