using Telegram.Bot.Types.ReplyMarkups;
using FloristAI.Application.Language;
using FloristAI.Adapter.Models;

namespace FloristAI.Adapter
{
    /// <summary>
    /// Адаптер обработки команды выбора языка интерфейса Telegram-бота.
    /// </summary>
    public class SelectLanguageAdapter : IMessageAdapter
    {
        /// <summary>
        /// Сервис для получения списка доступных языков.
        /// </summary>
        private readonly ILanguageService _languageService;

        /// <summary>
        /// Ключ маршрута, соответствующий команде.
        /// </summary>
        public string RouteKey => "start";

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="SelectLanguageAdapter"/>.
        /// </summary>
        /// <param name="languageService">Сервис для получения списка поддерживаемых языков.</param>
        public SelectLanguageAdapter(ILanguageService languageService)
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

            var language = await _languageService.GetLanguageList(chatId);

            var keyboard = new InlineKeyboardMarkup(
                language.Select(lang =>
                    new[] {
                        InlineKeyboardButton.WithCallbackData(
                            text: lang.Name,
                            callbackData: $"select_role:{lang.Code}" // например, "select_language:ru"
                        )
                    }).ToArray()
            );

            return new MessageResult
            {
                Text = "🌐 Выберите язык / Selectați o limbă",
                ReplyMarkup = keyboard
            };
        }
    }
}
