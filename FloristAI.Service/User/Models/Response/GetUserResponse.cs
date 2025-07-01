using System;

namespace FloristAI.Application.User.Models.Response
{
    /// <summary>
    /// Ответ сервиса с данными пользователя.
    /// </summary>
    public class GetUserResponse
    {
        /// <summary>
        /// Идентификатор пользователя.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Код языка интерфейса пользователя. Может быть <c>null</c>, если язык не установлен.
        /// </summary>
        public string LanguageCode { get; set; }
    }
}
