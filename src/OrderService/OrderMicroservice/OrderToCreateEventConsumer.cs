using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using MessageContracts;

namespace OrderMicroservice;

public class OrderToCreateEventConsumer : IConsumer<IOrderToCreate>
{
    private readonly IOrderStorage _orderStorage;

    public OrderToCreateEventConsumer(IOrderStorage orderStorage)
    {
        _orderStorage = orderStorage;
    }
    
    public async Task Consume(ConsumeContext<IOrderToCreate> context)
    {
        var orderNumber = Guid.NewGuid();
        Console.WriteLine($"Creating order {orderNumber} for customer:");
        Console.WriteLine($"Customer: {context.Message.CustomerNumber}");
        context.Message.Pizzas.ForEach(item =>
        {
            Console.WriteLine($"Item: {item.Name}, Price: {item.Price}");
        });

        await _orderStorage.StoreOrder(new Order
        {
            Id = orderNumber,
            CustomerNumber = context.Message.CustomerNumber,
            Pizzas = context.Message.Pizzas.Select(x => new Pizza
            {
                PizzaId = x.PizzaId,
                Name = x.Name,
                Price = x.Price
            }).ToList(),
        });

        await context.Publish<IOrderCreated>(new
        {
            OrderNumber = orderNumber,
            OrderData = new
            {
                context.Message.CustomerNumber,
                context.Message.Pizzas
            }
        });
    }
}

public class OrderToCreateEventConsumerDefinition : ConsumerDefinition<OrderToCreateEventConsumer>
{
    public OrderToCreateEventConsumerDefinition()
    {
        // override the default endpoint name
        EndpointName = "order-service";

        // limit the number of messages consumed concurrently
        // this applies to the consumer only, not the endpoint
        ConcurrentMessageLimit = 8;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<OrderToCreateEventConsumer> consumerConfigurator)
    {
        // configure message retry with millisecond intervals
        endpointConfigurator.UseMessageRetry(r => r.Intervals(100,200,500,800,1000));

        // use the outbox to prevent duplicate events from being published
        endpointConfigurator.UseInMemoryOutbox();
    }
}

// x.SetKebabCaseEndpointNameFormatter();
// x.UsingRabbitMq((context, cfg) =>
// {
//     // cfg.Host("localhost");
//     // cfg.UseInMemoryOutbox();
//     // cfg.ReceiveEndpoint(serviceName,
//     //     c =>
//     //         c.Consumer<OrderToCreateEventConsumer>(c =>
//     //             c.UseMessageRetry(m => m.Interval(5, new TimeSpan(0, 0, 10))))
//     // );
// });