using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;


namespace FloristAI.Adapter.Models
{
    /// <summary>
    /// Модель сообщения
    /// </summary>
    public class MessageResult
    {
        /// <summary>
        /// Фото
        /// </summary>
        public PhotoContent? Photo { get; set; }

        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Кнопки сообщения
        /// </summary>
        public ReplyMarkup? ReplyMarkup { get; set; }

        /// <summary>
        /// Опциональный ключ маршрута, на который нужно сделать редирект
        /// </summary>
        public string? RedirectRouteKey { get; set; }
    }
}
