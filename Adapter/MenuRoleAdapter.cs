using FloristAI.Adapter.Models;
using FloristAI.Application.Language;
using FloristAI.Application.User;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter
{
    /// <summary>
    /// Адаптер для отображения клиентского меню в зависимости от выбранной роли.
    /// </summary>
    public class MenuRoleAdapter : IMessageAdapter
    {
        /// <summary>
        /// Ключ маршрута, по которому будет вызываться адаптер.
        /// </summary>
        public string RouteKey => "role_menu";

        /// <summary>
        /// Сервис для получения данных по пользователю.
        /// </summary>
        private readonly IUserService _userService;

        /// <summary>
        /// Сервис для получения перевода сообщения.
        /// </summary>
        private readonly ILocalizationService _localizationService;

        /// <summary>
        /// Создаёт новый экземпляр <see cref="MenuRoleAdapter"/>.
        /// </summary>
        /// <param name="userService">Сервис для работы с пользователями.</param>
        /// <param name="localizationService">Сервис локализации текста.</param>
        public MenuRoleAdapter(IUserService userService, ILocalizationService localizationService)
        {
            _userService = userService;
            _localizationService = localizationService;
        }

        /// <summary>
        /// Обрабатывает входящее сообщение и возвращает клиентское меню.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="chatId">Идентификатор Telegram-чата пользователя.</param>
        /// <returns>Объект <see cref="MessageResult"/> с текстом меню и кнопками.</returns>
        public async Task<MessageResult> ProcessMessage(string parameter, long chatId)
        {
            var user = await _userService.GetUser(chatId);

            var keyboard = new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    text: _localizationService.GetString("Flower", user.LanguageCode),
                    callbackData: "get_menu:flower"
                ),
                InlineKeyboardButton.WithCallbackData(
                    text: _localizationService.GetString("Basket", user.LanguageCode),
                    callbackData: "get_menu:basket"
                ),
                InlineKeyboardButton.WithCallbackData(
                    text: _localizationService.GetString("Bouquet", user.LanguageCode),
                    callbackData: "get_menu:bouquet"
                ),
                InlineKeyboardButton.WithCallbackData(
                    text: _localizationService.GetString("Create_Bouquet", user.LanguageCode),
                    callbackData: "get_menu:createBouquet"
                ),
                InlineKeyboardButton.WithCallbackData(
                    text: _localizationService.GetString("My_Orders", user.LanguageCode),
                    callbackData: "get_menu:myOrder"
                ),
                InlineKeyboardButton.WithCallbackData(
                    text: _localizationService.GetString("Become_Partner", user.LanguageCode),
                    callbackData: "get_menu:becomePartner"
                ),
                InlineKeyboardButton.WithCallbackData(
                    text: _localizationService.GetString("Become_Partner", user.LanguageCode),
                    callbackData: "select_role"
                ),
            };

            return new MessageResult
            {
                Text = _localizationService.GetString("Menu_Client", user.LanguageCode),
                ReplyMarkup = keyboard
            };
        }
    }
}
