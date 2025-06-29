using System;

namespace FloristAI.Application.User.Models.Response
{
    /// <summary>
    /// Ответ сервиса при изменении языка интерфейса пользователя.
    /// </summary>
    public class EditLanguageInterfaceUserResponse
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Код выбранного языка интерфейса (например, "ru", "ro").
        /// </summary>
        public string LanguageCode { get; set; }
    }
}
