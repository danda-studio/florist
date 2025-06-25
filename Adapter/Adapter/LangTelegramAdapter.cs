using Telegram.Bot.Types.ReplyMarkups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FloristAI.Adapter.Adapter.Models;
using FloristAI.Application.Language;

namespace FloristAI.Adapter.Adapter
{
    /// <summary>
    /// Адаптер обработки команды выбора языка интерфейса Telegram-бота.
    /// </summary>
    public class LangTelegramAdapter : IMessageAdapter
    {
        /// <summary>
        /// Сервис для получения списка доступных языков.
        /// </summary>
        private readonly ILanguageService _languageService;

        /// <summary>
        /// Ключ маршрута, соответствующий команде "/start".
        /// </summary>
        public string RouteKey => "start";

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="LangTelegramAdapter"/>.
        /// </summary>
        /// <param name="languageService">Сервис для получения списка поддерживаемых языков.</param>
        public LangTelegramAdapter(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        /// <summary>
        /// Обрабатывает сообщение и возвращает список языков в виде инлайн-кнопок.
        /// </summary>
        /// <param name="message">Входящее сообщение пользователя.</param>
        /// <param name="chatId">Идентификатор чата, откуда пришло сообщение.</param>
        /// <returns>
        /// Объект <see cref="MessageResult"/>, содержащий текст и клавиатуру с выбором языка.
        /// </returns>
        public async Task<MessageResult> ProcessMessage(string message, long chatId)
        {
            var language = await _languageService.GetLanguageList();

            var keyboard = new InlineKeyboardMarkup(
                language.Select(lang =>
                    new[] {
                        InlineKeyboardButton.WithCallbackData(
                            text: lang.Name,
                            callbackData: $"select_language:{lang.Code}" // например, "select_language:ru"
                        )
                    }).ToArray()
            );

            return new MessageResult
            {
                Text = "Выберите язык / Selectați o limbă",
                ReplyMarkup = keyboard
            };
        }
    }
}
