using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FloristAI.Infrastructure
{
    /// <summary>
    /// Фабрика контекста базы данных для использования во время проектирования (миграций).
    /// </summary>
    public class DesignTimePostgresDbContextFactory : IDesignTimeDbContextFactory<PostgresDbContext>
    {
        /// <summary>
        /// Создаёт экземпляр <see cref="PostgresDbContext"/> с конфигурацией из файла appsettings.json.
        /// Этот метод используется инструментами EF Core во время миграций и других операций проектирования.
        /// </summary>
        /// <param name="args">Аргументы командной строки (не используются).</param>
        /// <returns>Экземпляр <see cref="PostgresDbContext"/> с настроенным подключением.</returns>
        public PostgresDbContext CreateDbContext(string[] args)
        {
            // Указываем путь к appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PostgresDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseNpgsql(connectionString);

            return new PostgresDbContext(optionsBuilder.Options);
        }
    }
}
