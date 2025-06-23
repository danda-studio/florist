using Adapter;
using FloristAI.Application;
using FloristAI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<PostgresDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("FloristAI.Infrastructure")));

builder.Services.AddScoped<ILanguageService, LanguageService>();

builder.Services.AddSingleton<ITelegramBotClient>(provider =>
{
    var token = builder.Configuration["Telegram:Token"];
    return new TelegramBotClient(token);
});

builder.Services.AddScoped<IMessageAdapter, LangTelegramAdapter>();

//builder.Services.AddHostedService<BotWorker>();

await builder.Build().RunAsync();
