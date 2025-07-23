using FloristAI.Adapter;
using FloristAI.Adapter.ClientMenuBuilder;
using FloristAI.Adapter.ClientMenuBuilder.BecomePartnerStep;
using FloristAI.Adapter.PartnerMenuBuilder;
using FloristAI.Adapter.RoleMenuBuilder;
using FloristAI.Adapter.StepFlowBuilder;
using FloristAI.Adapter.StepMenuBuilder;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using FloristAI.Core.Store;
using FloristAI.Infrastructure;
using FloristAI.Infrastructure.Persistence;
using FloristAI.Router;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
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


            var redisConnectionString = Environment.GetEnvironmentVariable("RedisConnection")
                ?? throw new Exception("RedisConnection переменная окружения не найдена");

            // Регистрация Redis
            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(redisConnectionString));

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
            services.AddScoped<IMessageAdapter, StepMessageAdapter>();
            services.AddScoped<IMessageAdapter, StepTextInputAdapter>();
            services.AddScoped<IRoleMenuBuilder, ClientMenuBuilder>();
            services.AddScoped<IRoleMenuBuilder, PartnerMenuBuilder>();
            services.AddScoped<IStepMenuBuilder, BecomePartnerMenuBuilder>();
            services.AddScoped<IStepMenuBuilder, PartnerMenuStepReferralUrl>();
            services.AddScoped<IStepMenuBuilder, PartnerMenuStepReporting>();
            services.AddScoped<IStepFlowBuilder, BecomePartnerStepFirstName>();
            services.AddScoped<IStepFlowBuilder, BecomePartnerStepLastName>();
            services.AddScoped<IStepFlowBuilder, BecomePartnerStepPhone>();
            services.AddScoped<IStepFlowBuilder, BecomePartnerStepFinal>();
            services.AddScoped<IRoleMenuBuilderProvider, RoleMenuBuilderProvider>();
            services.AddScoped<IStepMenuProvider, StepMenuProvider>();
            services.AddScoped<IStepFlowProvider, StepFlowProvider>();
            services.AddScoped<IStepInitializer, StepInitializer>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICacheRepository, CacheRepository>();

            services.AddScoped<Lazy<IStepFlowProvider>>(sp =>
                new Lazy<IStepFlowProvider>(() => sp.GetRequiredService<IStepFlowProvider>()));

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
