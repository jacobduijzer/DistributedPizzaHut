using FrontendApplication.TestApplication;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

const string serviceName = "frontend-service";
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
                            c.Consumer<PaymentToMakeEventConsumer>(c =>
                                c.UseMessageRetry(m => m.Interval(5, new TimeSpan(0, 0, 10))))
                    );
                });
            })
            .AddSingleton<CreateOrderCommand>()
            .AddOpenTelemetryTracing(builder =>
            {
                builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(typeof(Program).Assembly.GetName().Name))
                    .AddMassTransitInstrumentation()
                    // .AddAspNetCoreInstrumentation()
                    // .AddHttpClientInstrumentation()
                    .AddConsoleExporter()
                    .AddZipkinExporter(with => with.Endpoint = new Uri(zipkinEndpoint));
            })
            .AddHostedService<ConsoleHostedService>())
    .RunConsoleAsync();