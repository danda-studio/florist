using FloristAI.Core.Entities.Enums;
using FloristAI.Core.Entities.UserInfo;
using FloristAI.Core.Store;
using Microsoft.EntityFrameworkCore;

namespace FloristAI.Infrastructure.Persistence
{
    /// <summary>
    /// Репозиторий для работы с пользователями в базе данных PostgreSQL.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly PostgresDbContext _dbContext;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UserRepository"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        public UserRepository(PostgresDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Получает список ролей пользователя по его идентификатору.
        /// </summary>
        /// <param name="Id">Идентификатор пользователя.</param>
        /// <returns>Список ролей пользователя.</returns>
        public async Task<List<UserRole>> GetRoles(int Id)
        {
            return await _dbContext.UserRoles
                .Where(ur => ur.UserId == Id)
                .ToListAsync();
        }

        /// <summary>
        /// Получает пользователя по Telegram chatId.
        /// </summary>
        /// <param name="chatId">Идентификатор чата в Telegram.</param>
        /// <returns>Пользователь или null, если не найден.</returns>
        public async Task<User?> GetUserByChatId(long chatId)
        {
            return await _dbContext.Users
                .Include(u => u.TgData)
                .FirstOrDefaultAsync(u => u.TgData != null && u.TgData.TelegramId == chatId);
        }

        /// <summary>
        /// Создаёт пользователя с данными Telegram и языком интерфейса.
        /// </summary>
        /// <param name="chatId">Идентификатор чата Telegram.</param>
        /// <param name="languageCode">Код языка интерфейса.</param>
        /// <returns>Созданный пользователь.</returns>
        public async Task<User> CreateUserWithChatData(long chatId, string languageCode)
        {
            var user = new User
            {
                LanguageCode = languageCode,
                TgData = new UserTgData
                {
                    TelegramId = chatId
                },
                Roles = new List<UserRole>
                {
                    new UserRole
                    {
                        Role = RoleType.Client
                    }
                }
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Изменяет язык интерфейса пользователя.
        /// </summary>
        /// <param name="Id">Идентификатор пользователя.</param>
        /// <param name="languageCode">Новый код языка интерфейса.</param>
        /// <returns>True, если обновление успешно, иначе false.</returns>
        public async Task<bool> EditLanguageCode(int Id, string languageCode)
        {
            var user = await _dbContext.Users.FindAsync(Id);
            if (user == null) return false;

            user.LanguageCode = languageCode;
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
