
namespace FloristAI.Application.Users.Models.Response
{
    /// <summary>
    /// Ответ сервиса при создании нового пользователя или получении существующего.
    /// </summary>
    public class AddUserResponse
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Код языка интерфейса пользователя. Может быть <c>null</c>, если язык не установлен.
        /// </summary>
        public string LanguageCode { get; set; } = "ru";
    }
}
