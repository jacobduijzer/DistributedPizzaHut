using System;
using System.Linq;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace OrderMicroservice.Specs;

public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup: class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptors = services
                .Where(d => d.ServiceType.Namespace.Contains("MassTransit",StringComparison.OrdinalIgnoreCase))
                .ToList();
            foreach (var d in descriptors) 
            {
                services.Remove(d);
            }    

            var hostedService = services.SingleOrDefault(s => s.ImplementationType == typeof(ConsoleHostedService));
            services.Remove(hostedService);

            var orderDbContext = services.SingleOrDefault(x => x.ImplementationType == typeof(OrderDbContext));
            services.Remove(orderDbContext);

            services.AddDbContext<OrderDbContext>(options => options.UseInMemoryDatabase("Orders"));
            services.AddMassTransitInMemoryTestHarness(cfg => { cfg.AddConsumer<OrderToCreateEventConsumer>(typeof(OrderToCreateEventConsumerDefinition)); });
            services.BuildServiceProvider(true);
        });
    }
}