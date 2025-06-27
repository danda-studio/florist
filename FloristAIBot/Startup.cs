using FloristAI.Adapter;
using FloristAI.Application.Language;
using FloristAI.Application.User;
using FloristAI.Core.Store;
using FloristAI.Infrastructure;
using FloristAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Router;
using Telegram.Bot;

namespace FloristAIBot
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString;

            if (_env.IsProduction())
            {
                // В продакшене берём из переменных окружения
                var host = Environment.GetEnvironmentVariable("Host") ?? throw new Exception("Host не найдена");
                var port = Environment.GetEnvironmentVariable("Port") ?? throw new Exception("Port не найдена");
                var database = Environment.GetEnvironmentVariable("Database") ?? throw new Exception("Database не найдена");
                var username = Environment.GetEnvironmentVariable("Username") ?? throw new Exception("Username не найдена");
                var password = Environment.GetEnvironmentVariable("Password") ?? throw new Exception("Password не найдена");

                connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";
                
            }
            else
            {

                connectionString = _configuration.GetConnectionString("DefaultConnection")
                                   ?? throw new Exception("Connection string не найдена в конфигурации");
            }

            services.AddDbContext<PostgresDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Telegram-бот и бизнес-логика
            services.AddHostedService<BotWorker>(provider =>
                 new BotWorker(
                     provider.GetRequiredService<ITelegramBotClient>(),
                     provider.GetRequiredService<AdapterRouter>()));

            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILocalizationService, JsonLocalizationService>();
            services.AddScoped<IMessageAdapter, LangTelegramAdapter>();
            services.AddScoped<IMessageAdapter, RoleTelegramAdapter>();
            services.AddScoped<IUserRepository, UserRepository>();
            

            var token = Environment.GetEnvironmentVariable("Bot_token") ?? _configuration["Telegram:Token"];
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
