using Adapter;
using FloristAI.Application;
using FloristAI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Router;
using Telegram.Bot;

namespace FloristAIBot
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // PostgreSQL
            var host = Environment.GetEnvironmentVariable("Host");
            var port = Environment.GetEnvironmentVariable("Port");
            var database = Environment.GetEnvironmentVariable("Database");
            var username = Environment.GetEnvironmentVariable("Username");
            var password = Environment.GetEnvironmentVariable("Password");

            var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";

            services.AddDbContext<PostgresDbContext>(options =>
                options.UseNpgsql(connectionString,
                    b => b.MigrationsAssembly("FloristAI.Infrastructure")));

            // Telegram-бот и бизнес-логика
            services.AddHostedService<BotWorker>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<IMessageAdapter, LangTelegramAdapter>();
            services.AddScoped<AdapterRouter>();

            var token = Environment.GetEnvironmentVariable("Bot_token");
            services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token));

            // Заглушка для Render
            services.AddRouting();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Заглушка для Render
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Bot is running.");
                });
            });
        }
    }
}
