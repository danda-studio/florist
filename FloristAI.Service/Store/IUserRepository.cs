using FloristAI.Core.Entities.ReferralsAndPartners;
using FloristAI.Core.Entities.UserInfo;

namespace FloristAI.Core.Store
{
    /// <summary>
    /// Интерфейс репозитория для работы с пользователями и их ролями.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Получает список ролей пользователя по идентификатору.
        /// </summary>
        /// <param name="Id">Идентификатор пользователя.</param>
        /// <returns>Список ролей пользователя.</returns>
        Task<List<UserRole>> GetRoles(int Id);

        /// <summary>
        /// Создаёт нового пользователя с данными чата Telegram и кодом языка интерфейса.
        /// </summary>
        /// <param name="chatId">Идентификатор чата Telegram.</param>
        /// <param name="languageCode">Код языка интерфейса.</param>
        /// <returns>Созданный пользователь.</returns>
        Task<User> CreateUserWithChatData(long chatId, string languageCode);

        /// <summary>
        /// Получает пользователя по идентификатору чата Telegram.
        /// </summary>
        /// <param name="chatId">Идентификатор чата Telegram.</param>
        /// <returns>Пользователь или null, если не найден.</returns>
        Task<User?> GetUserByChatId(long chatId);

        /// <summary>
        /// Обновляет код языка интерфейса пользователя.
        /// </summary>
        /// <param name="Id">Идентификатор пользователя.</param>
        /// <param name="languageCode">Новый код языка интерфейса.</param>
        /// <returns>True, если обновление прошло успешно, иначе false.</returns>
        Task<bool> EditLanguageCode(int Id, string languageCode);

        Task<Partner> AddPartner(Partner partner);

        Task<bool> IsPartner(long chatId);
        Task<string?> GetSpreadsheetId(int userId);
        Task<Referal> AddReferal(Referal referal, int partnerId);

    }
}
