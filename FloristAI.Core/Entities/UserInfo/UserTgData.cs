namespace FloristAI.Core.Entities.UserInfo
{
    /// <summary>
    /// Модель информации пользователя с Telegram
    /// </summary>
    public class UserTgData
    {
        /// <summary>
        /// Идентификатор пользователя во внутренней системе.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Уникальный идентификатор пользователя в Telegram.
        /// </summary>
        public long TelegramId { get; set; }

        /// <summary>
        /// Имя пользователя в Telegram.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Телефон пользователя, связанный с Telegram-аккаунтом.
        /// </summary>
        public string Phone { get; set; } = string.Empty;


        /// <summary>
        /// Язык интерфейса пользователя, связанный с Telegram-аккаунтом.
        /// </summary>
        public string LanguageCode { get; set; } = "ru";

        /// <summary>
        /// Пользователь, к которому относятся данные Telegram.
        /// </summary>
        public User User { get; set; }
    }
}
