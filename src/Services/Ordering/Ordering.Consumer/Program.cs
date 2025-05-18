using MassTransit;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationServices();  // ? ????????? ???? ????????
        services.AddInfrastructureServices(context.Configuration); // ? DbContext ? ?????

        services.AddMassTransit(x =>
        {
            x.AddConsumer<BasketCheckoutConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ReceiveEndpoint("BasketCheckout", e =>
                {
                    e.ConfigureConsumer<BasketCheckoutConsumer>(ctx);
                });
            });
        });

        services.AddMassTransitHostedService();
    })
    .Build();

await host.RunAsync();
