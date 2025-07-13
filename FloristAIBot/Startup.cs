using FloristAI.Adapter;
using FloristAI.Adapter.ClientMenuBuilder;
using FloristAI.Adapter.RoleMenuBuilder;
using FloristAI.Adapter.StepMenuBuilder;
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
    /// <summary>
    /// Класс настройки сервисов и конвейера обработки запросов
    /// </summary>
    public class Startup
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Конструктор класса <see cref="Startup"/>.
        /// </summary>
        /// <param name="configuration">Объект конфигурации приложения.</param>
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Метод регистрации сервисов в контейнере внедрения зависимостей.
        /// </summary>
        /// <param name="services">Коллекция сервисов.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // В продакшене берём из переменных окружения
            var host = Environment.GetEnvironmentVariable("Host") ?? throw new Exception("Host не найдена");
            var port = Environment.GetEnvironmentVariable("Port") ?? throw new Exception("Port не найдена");
            var database = Environment.GetEnvironmentVariable("Database") ?? throw new Exception("Database не найдена");
            var username = Environment.GetEnvironmentVariable("Username") ?? throw new Exception("Username не найдена");
            var password = Environment.GetEnvironmentVariable("Password") ?? throw new Exception("Password не найдена");

            string connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";

            services.AddDbContext<PostgresDbContext>(options =>
                options.UseNpgsql(connectionString,
                 b => b.MigrationsAssembly("FloristAI.Infrastructure")));

            // Регистрация Telegram-бота и бизнес-логики
            services.AddHostedService<BotWorker>();
            services.AddScoped<AdapterRouter>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILocalizationService, JsonLocalizationService>();
            services.AddScoped<IMessageAdapter, SelectLanguageAdapter>();
            services.AddScoped<IMessageAdapter, SelectRoleAdapter>();
            services.AddScoped<IMessageAdapter, MenuRoleAdapter>();
            services.AddScoped<IMessageAdapter, StepMenuAdapter>();
            services.AddScoped<IRoleMenuBuilder, ClientMenuBuilder>();
            services.AddScoped<IStepMenuBuilder, BecomePartnerMenuBuilder>();
            services.AddScoped<IRoleMenuBuilderProvider, RoleMenuBuilderProvider>();
            services.AddScoped<IStepMenuProvider, StepMenuProvider>();
            services.AddScoped<IUserRepository, UserRepository>();

            var token = Environment.GetEnvironmentVariable("Bot_token") ?? _configuration["Telegram:Token"];
            services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token));

            // Заглушка для Render
            services.AddRouting();
        }

        /// <summary>
        /// Метод конфигурации HTTP конвейера приложения.
        /// </summary>
        /// <param name="app">Приложение для конфигурации конвейера запросов.</param>
        /// <param name="env">Среда выполнения веб-приложения.</param>
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
