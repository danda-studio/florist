using FloristAI.Adapter;
using FloristAI.Adapter.AdminMenuBuilder;
using FloristAI.Adapter.AdminMenuBuilder.GenerateInviteLinkPartnerStep;
using FloristAI.Adapter.ClientMenuBuilder;
using FloristAI.Adapter.ClientMenuBuilder.BecomePartnerStep;
using FloristAI.Adapter.PartnerMenuBuilder;
using FloristAI.Adapter.RoleMenuBuilder;
using FloristAI.Adapter.StepFlowBuilder;
using FloristAI.Adapter.StepMenuBuilder;
using FloristAI.Application.GoogleDrive;
using FloristAI.Application.GoogleSheets;
using FloristAI.Application.Language;
using FloristAI.Application.Store;
using FloristAI.Application.Users;
using FloristAI.Core.Store;
using FloristAI.Infrastructure;
using FloristAI.Infrastructure.Persistence;
using FloristAI.Router;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
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
            string connectionString;
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables() 
                .Build();

            var host = Environment.GetEnvironmentVariable("Host");
            var port = Environment.GetEnvironmentVariable("Port");
            var database = Environment.GetEnvironmentVariable("Database");
            var username = Environment.GetEnvironmentVariable("Username");
            var password = Environment.GetEnvironmentVariable("Password");
            if (!string.IsNullOrEmpty(host))
            {
                connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password}";
            }
            else
            {
                // Берем из appsettings.json
                connectionString = configuration.GetConnectionString("DefaultConnection")
                    ?? throw new Exception("Не найдена строка подключения");
            }


            services.AddDbContext<PostgresDbContext>(options =>
                options.UseNpgsql(connectionString,
                 b => b.MigrationsAssembly("FloristAI.Infrastructure")));


            var redisConnectionString = Environment.GetEnvironmentVariable("RedisConnection");
            if(string.IsNullOrEmpty(redisConnectionString))
            {
                // Берем из appsettings.json
                redisConnectionString = configuration.GetConnectionString("RedisConnection")
                    ?? throw new Exception("Не найдена строка подключения к Redis");
            }

            // Регистрация Redis
            services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(redisConnectionString));

            services.AddSingleton(provider =>
            {
                var credential = GetOAuthCredential().Result;
                //var credential = GetServiceAccountCredential();

                return new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "FloristAI"
                });
            });

            services.AddSingleton(provider =>
            {
                var credential = GetOAuthCredential().Result;
                //var credential = GetServiceAccountCredential();

                return new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "FloristAI"
                });
            });

            static async Task<UserCredential> GetOAuthCredential()
            {
                using var stream = new FileStream("client_secret_2_845291149186-prkk1vvk046gn50illkjhmvekpb19h0g.apps.googleusercontent.com.json", FileMode.Open, FileAccess.Read);

                var credPath = Path.Combine(Directory.GetCurrentDirectory(), "token.json");

                var googleSecrets = await GoogleClientSecrets.FromStreamAsync(stream);

                return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    googleSecrets.Secrets,
                    new[] { DriveService.Scope.Drive, SheetsService.Scope.Spreadsheets },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));
            }


            // Регистрация Telegram-бота и бизнес-логики
            services.AddHostedService<BotWorker>();
            services.AddScoped<AdapterRouter>();
            services.AddScoped<IGoogleSheets, GoogleSheets>();
            services.AddScoped<IGoogleDrive, GoogleDrive>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IGoogleSheetsService, GoogleSheetsService>();
            services.AddScoped<IGoogleDriveService, GoogleDriveService>();
            services.AddScoped<ILocalizationService, JsonLocalizationService>();
            services.AddScoped<IMessageAdapter, SelectLanguageAdapter>();
            services.AddScoped<IMessageAdapter, SelectRoleAdapter>();
            services.AddScoped<IMessageAdapter, MenuRoleAdapter>();
            services.AddScoped<IMessageAdapter, StepMenuAdapter>();
            services.AddScoped<IMessageAdapter, StepMessageAdapter>();
            services.AddScoped<IMessageAdapter, StepTextInputAdapter>();
            services.AddScoped<IRoleMenuBuilder, ClientMenuBuilder>();
            services.AddScoped<IRoleMenuBuilder, PartnerMenuBuilder>();
            services.AddScoped<IRoleMenuBuilder, AdminMenuBuilder>();
            services.AddScoped<IStepMenuBuilder, BecomePartnerMenuBuilder>();
            services.AddScoped<IStepMenuBuilder, PartnerMenuStepReferralUrl>();
            services.AddScoped<IStepMenuBuilder, PartnerMenuStepReporting>();
            services.AddScoped<IStepMenuBuilder, AdminReporting>();
            services.AddScoped<IStepFlowBuilder, GenerateInviteLinkPartnerStepFirstNamePartner>();
            services.AddScoped<IStepFlowBuilder, GenerateInviteLinkPartnerStepLastNamePartner>();
            services.AddScoped<IStepFlowBuilder, GenerateInviteLinkPartnerStepPhonePartner>();
            services.AddScoped<IStepFlowBuilder, GenerateInviteLinkPartnerStepFinalPartner>();
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

            var token = Environment.GetEnvironmentVariable("Bot_token");
            if (string.IsNullOrEmpty(token))
            {
                // Берем из appsettings.json
                token = configuration["Telegram:Token"]
                    ?? throw new Exception("Не найден токен бота");
            }
            services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token));

            // Заглушка для Render
            services.AddRouting();
        }



        private static GoogleCredential GetServiceAccountCredential()
        {
            var keyFilePath = Path.Combine(AppContext.BaseDirectory, "kisaflori-8a9b4bc9ff09.json");

            return GoogleCredential.FromFile(keyFilePath)
                .CreateScoped(new[]
                {
                DriveService.Scope.Drive,
                SheetsService.Scope.Spreadsheets
                });
        }

        /// <summary>
        /// Метод конфигурации HTTP конвейера приложения.
        /// </summary>
        /// <param name="app">Приложение для конфигурации конвейера запросов.</param>
        /// <param name="env">Среда выполнения веб-приложения.</param>
        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        //{
        //    // Заглушка для Render
        //    app.UseRouting();

        //    app.UseEndpoints(endpoints =>
        //    {
        //        endpoints.MapGet("/", async context =>
        //        {
        //            await context.Response.WriteAsync("Bot is running.");
        //        });
        //    });
        //}
    }
}
