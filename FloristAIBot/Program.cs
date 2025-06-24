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
var host = Environment.GetEnvironmentVariable("Host");
var port = Environment.GetEnvironmentVariable("Port");
var database = Environment.GetEnvironmentVariable("Database");
var username = Environment.GetEnvironmentVariable("Username");
var password = Environment.GetEnvironmentVariable("Password");

var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";

builder.Services.AddDbContext<PostgresDbContext>(options =>
    options.UseNpgsql(connectionString,
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
    var token = Environment.GetEnvironmentVariable("Bot_token");
    return new TelegramBotClient(token);
});

/// <summary>
/// Запускает приложение.
/// </summary>
await builder.Build().RunAsync();
