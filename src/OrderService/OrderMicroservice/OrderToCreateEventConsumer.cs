using MassTransit;
using MessageContracts;

namespace OrderMicroservice;

public class OrderToCreateEventConsumer : IConsumer<IOrderToCreate>
{
    public async Task Consume(ConsumeContext<IOrderToCreate> context)
    {
        var orderNumber = Guid.NewGuid();
        Console.WriteLine($"Creating order {orderNumber} for customer:");
        Console.WriteLine($"Customer: {context.Message.CustomerNumber}");
        context.Message.Pizzas.ForEach(item =>
        {
            Console.WriteLine($"Item: {item.Name}, Price: {item.Price}");
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