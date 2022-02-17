using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OrderMicroservice;

const string serviceName = "order-service";
const string zipkinEndpoint = "http://localhost:9411/api/v2/spans";

await Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.AddMassTransit(x =>
            {
                x.SetKebabCaseEndpointNameFormatter();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost");
                    cfg.UseInMemoryOutbox();
                    cfg.ReceiveEndpoint(serviceName,
                        c =>
                            c.Consumer<OrderToCreateEventConsumer>(c =>
                                c.UseMessageRetry(m => m.Interval(5, new TimeSpan(0, 0, 10))))
                    );
                });
            })
            .AddOpenTelemetryTracing(builder =>
            {
                builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService(typeof(Program).Assembly.GetName().Name))
                    .AddMassTransitInstrumentation()
                    .AddConsoleExporter()
                    .AddZipkinExporter(with => with.Endpoint = new Uri(zipkinEndpoint));
            }).AddHostedService<ConsoleHostedService>())
    .RunConsoleAsync();