using Adapter;
using FloristAI.Application;
using FloristAI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Router;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

/// <summary>
/// Регистрирует контекст базы данных PostgreSQL с миграциями в сборке FloristAI.Infrastructure.
/// </summary>
builder.Services.AddDbContext<PostgresDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("FloristAI.Infrastructure")));

/// <summary>
/// Регистрирует фоновой сервис для работы Telegram-бота.
/// </summary>
builder.Services.AddHostedService<BotWorker>();

/// <summary>
/// Регистрирует сервисы и адаптеры для обработки языков и маршрутизации сообщений.
/// </summary>
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<IMessageAdapter, LangTelegramAdapter>();
builder.Services.AddScoped<AdapterRouter>();

/// <summary>
/// Регистрирует TelegramBotClient с токеном из конфигурации.
/// </summary>
builder.Services.AddSingleton<ITelegramBotClient>(provider =>
{
    var token = builder.Configuration["Telegram:Token"];
    return new TelegramBotClient(token);
});

/// <summary>
/// Запускает приложение.
/// </summary>
await builder.Build().RunAsync();
