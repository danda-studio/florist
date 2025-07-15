using FloristAI.Adapter.Models;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using FloristAI.Core.Entities.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter
{
    /// <summary>
    /// Адаптер, обрабатывающий выбор роли пользователя и перенаправляющий к соответствующему меню.
    /// </summary>
    public class SelectRoleAdapter : IMessageAdapter
    {
        /// <summary>
        /// Ключ маршрута для данного адаптера.
        /// </summary>
        public string RouteKey => "select_role";

        /// <summary>
        /// Сервис для получения данных по пользователю.
        /// </summary>
        private readonly IUserService _userService;

        /// <summary>
        /// Сервис для получения перевода сообщения.
        /// </summary>
        private readonly ILocalizationService _localizationService;

        /// <summary>
        /// Создаёт экземпляр <see cref="SelectRoleAdapter"/>.
        /// </summary>
        /// <param name="userService">Сервис пользователей.</param>
        /// <param name="localizationService">Сервис локализации.</param>
        public SelectRoleAdapter(IUserService userService, ILocalizationService localizationService)
        {
            _userService = userService;
            _localizationService = localizationService;
        }

        /// <summary>
        /// Обрабатывает сообщение Telegram, определяет роли пользователя и отображает меню выбора или делает редирект.
        /// </summary>
        /// <param name="parameter">Код языка, выбранный пользователем.</param>
        /// <param name="chatId">Идентификатор чата пользователя.</param>
        /// <returns>Сообщение с результатом: кнопки выбора ролей или редирект.</returns>
        public async Task<MessageResult> ProcessMessage(string parameter, long chatId)
        {
            var role = await _userService.GetRolesByTelegramId(chatId, parameter);
            var langCode = await _userService.EditLanguageInterfaceUser(chatId, parameter);

            if (role.Roles.Count == 1)
            {
                var onlyRole = role.Roles.First();

                string redirectKey = GetRouteKeyForRole(onlyRole.RoleType);

                return new MessageResult
                {
                    Text = parameter == "ru"
                        ? $"Ваша роль:{onlyRole.RoleName}. Открываю меню..."
                        : $"Rolul dvs.: {onlyRole.RoleName}. Se deschide meniul...",
                    RedirectRouteKey = redirectKey
                };
            }

            var buttons = role.Roles.Select(r => new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    text: r.RoleName,
                    callbackData: $"role_menu:{r.RoleType}"
                )
            }).ToArray();

            var languageButton = new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    text: _localizationService.GetString("LanguageSelection", parameter),
                    callbackData: "start"
                )
            };

            var keyboard = buttons.Append(languageButton).ToArray();

            return new MessageResult
            {
                Text = role.Roles.Count > 1
                    ? (parameter == "ru"
                        ? "Вам доступно несколько ролей:"
                        : "Aveți acces la mai multe roluri:")
                    : (parameter == "ru"
                        ? "Ваша роль:"
                        : "Rolul dvs.:"),
                ReplyMarkup = new InlineKeyboardMarkup(keyboard)
            };
        }

        /// <summary>
        /// Возвращает ключ маршрута (callbackData) для указанной роли.
        /// </summary>
        /// <param name="role">Тип роли пользователя.</param>
        /// <returns>Строка callback-команды, соответствующая роли.</returns>
        private static string GetRouteKeyForRole(RoleType role)
        {
            return role switch
            {
                RoleType.Client => "role_menu:Сlient",
                RoleType.Admin => "role_menu:Admin",
                RoleType.Partner => "role_menu:Partner",
                RoleType.Florist => "role_menu:Florist",
                RoleType.Moderator => "role_menu:Moderator",
                _ => "unknown_role"
            };
        }
    }
}
