using FloristAIBot;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var startup = new Startup(context.Configuration);
        startup.ConfigureServices(services);
    })
    .Build()
    .Run();