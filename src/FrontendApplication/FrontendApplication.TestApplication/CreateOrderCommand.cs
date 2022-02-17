using MassTransit;
using MessageContracts;

namespace FrontendApplication.TestApplication;

public class CreateOrderCommand
{
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateOrderCommand(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle()
    {
        await _publishEndpoint.Publish<IOrderToCreate>(new
        {
            CustomerNumber = Guid.NewGuid(),
            Pizzas = new List<Pizza>()
            {
                new Pizza
                {
                    PizzaId = Guid.NewGuid(),
                    Name = "Some Pizza",
                    Price = 5.95m
                },
                new Pizza
                {
                    PizzaId = Guid.NewGuid(),
                    Name = "Another Pizza",
                    Price = 6.95m
                }
            }
        });
    }
}