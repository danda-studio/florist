using FloristAI.Adapter.Models;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using FloristAI.Application.Users.Models.Request;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

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

        private readonly IUserService _userService;

        /// <summary>
        /// Ключ маршрута, соответствующий команде.
        /// </summary>
        public string RouteKey => "start";

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="SelectLanguageAdapter"/>.
        /// </summary>
        /// <param name="languageService">Сервис для получения списка поддерживаемых языков.</param>
        public SelectLanguageAdapter(ILanguageService languageService, IUserService userService)
        {
            _languageService = languageService;
            _userService = userService;
        }

        /// <summary>
        /// Обрабатывает сообщение и возвращает список языков в виде инлайн-кнопок.
        /// </summary>
        /// <param name="message">Входящее сообщение пользователя.</param>
        /// <param name="chatId">Идентификатор чата, откуда пришло сообщение.</param>
        /// <returns>
        /// Объект <see cref="MessageResult"/>, содержащий текст и клавиатуру с выбором языка.
        /// </returns>
        public async Task<List<MessageResult>> ProcessMessage(MessageContext context)
        {
            var user = await _userService.GetOrCreateUser(context.ChatId, "ru");
            if (!string.IsNullOrEmpty(context.Parameter) && int.TryParse(context.Parameter, out int partnerId))
            {
                await _userService.ProcessReferral(new ProcessReferralRequest
                {
                    UserId = user.UserId,
                    PartnerId = partnerId

                });

            }

            var language = await _languageService.GetLanguageList(context.ChatId);

            var keyboard = new InlineKeyboardMarkup(
                language.Select(lang =>
                    new[] {
                        InlineKeyboardButton.WithCallbackData(
                            text: lang.Name,
                            callbackData: $"select_role:{lang.Code}" // например, "select_language:ru"
                        )
                    }).ToArray()
            );

            return new List<MessageResult>
            {
                new MessageResult
                {
                    Text = "🌐 Выберите язык / Selectați o limbă",
                    ReplyMarkup = keyboard
                }
            };
            
        }
    }
}
