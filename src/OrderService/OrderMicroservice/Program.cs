using GreenPipes;
using MassTransit;
using OrderMicroservice;

var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
    cfg.Host("localhost");
    cfg.ReceiveEndpoint("order-service", e =>
    {
        e.UseInMemoryOutbox();
        e.Consumer<OrderToCreateEventConsumer>(c =>
            c.UseMessageRetry(m => m.Interval(5, new TimeSpan(0, 0, 10))));
    });
});
var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
await busControl.StartAsync(source.Token);
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
    await busControl.StopAsync();
}