using FloristAI.Adapter.Models;
using FloristAI.Adapter.RoleMenuBuilder;
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
        private readonly IRoleMenuBuilderProvider _builderProvider;

        /// <summary>
        /// Создаёт новый экземпляр <see cref="MenuRoleAdapter"/>.
        /// </summary>
        /// <param name="userService">Сервис для работы с пользователями.</param>
        /// <param name="localizationService">Сервис локализации текста.</param>
        public MenuRoleAdapter(IUserService userService, IRoleMenuBuilderProvider builderProvider)
        {
            _userService = userService;
            _builderProvider = builderProvider;
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
            var builder = _builderProvider.GetBuilder(parameter);
            return await builder.BuildMenu(chatId);
        }
    }
}
