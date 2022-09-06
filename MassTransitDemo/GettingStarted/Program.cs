using GettingStarted;
using GettingStarted.RequestTest;
using MassTransit;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext,services) =>
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<MessageConsumer>();
            x.AddConsumer<CheckOrderStatusConsumer>();


            x.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });

            x.AddRequestClient<CheckOrderStatus>(new Uri("exchange:order-status"));

            //x.UsingRabbitMq((context, cfg) =>
            //{
            //    var connection = "amqp://lx:admin@ip:5672/my_vhost";//不加主机会报错
            //    cfg.Host(connection);

            //    cfg.ConfigureEndpoints(context);
               
            //});  //
        });
        //services.AddOptions<MassTransitHostOptions>()   //rabbitmq用 UsingInMemory 不需要
        //      .Configure(options =>
        //      {
        //          options.WaitUntilStarted = true;
        //      });
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
