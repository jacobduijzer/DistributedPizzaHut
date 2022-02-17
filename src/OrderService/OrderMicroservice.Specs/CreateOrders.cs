using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Testing;
using MessageContracts;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;
using Xunit;

namespace OrderMicroservice.Specs;

[Binding]
public class CreateOrders : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly ScenarioContext _scenarioContext;
    private readonly CustomWebApplicationFactory<Startup> _factory;

    public CreateOrders(ScenarioContext scenarioContext, CustomWebApplicationFactory<Startup> factory)
    {
        _scenarioContext = scenarioContext;
        _factory = factory;
    }

    [Given(@"Julia orders a (.*) with a price of (.*)")]
    [Given(@"she orders a (.*) with a price of (.*)")]
    public void GivenJuliaOrdersPizzaPepperoniForAPriceOf(string name, decimal price)
    {
        if (!_scenarioContext.TryGetValue("PIZZA_LIST", out List<Pizza> pizzas))
            pizzas = new List<Pizza>();

        pizzas.Add(new Pizza {Name = name, Price = price});
        _scenarioContext.Set(pizzas, "PIZZA_LIST");
    }

    [When(@"she finalizes her order")]
    public async Task WhenSheFinalizesHerOrder()
    {
        var customerId = Guid.NewGuid();
        _scenarioContext.Add("CUSTOMER_ID", customerId);
        var pizzas = _scenarioContext.Get<List<Pizza>>("PIZZA_LIST");
        var cancellationToken = new CancellationTokenSource(15000).Token;
        var harness = _factory.Services.GetRequiredService<InMemoryTestHarness>();
        await harness.Start(cancellationToken);

        var bus = _factory.Services.GetRequiredService<IBusControl>();
        try
        {
            await bus.Publish<IOrderToCreate>(new
            {
                CustomerNumber = customerId,
                Pizzas = pizzas
            }, cancellationToken);

            var consumed = await harness.Consumed.Any<IOrderToCreate>(cancellationToken);
            _scenarioContext.Add("EVENT_CONSUMED", consumed);
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Then(@"an order will be created")]
    public async Task ThenAnOrderWillBeCreated()
    {
        Assert.True(_scenarioContext.Get<bool>("EVENT_CONSUMED"));

        await using var scope = _factory.Services.CreateAsyncScope();
        var orderStorage = scope.ServiceProvider.GetRequiredService<IOrderStorage>();
        var order = await orderStorage.GetOrdersForCustomer(_scenarioContext.Get<Guid>("CUSTOMER_ID"));
        Assert.NotNull(order);
        Assert.True(order.Count == 1);
    }
}