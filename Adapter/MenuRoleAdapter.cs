using FloristAI.Adapter.Models;
using FloristAI.Adapter.RoleMenuBuilder;
using FloristAI.Adapter.StepMenuBuilder;
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
        /// Сервис для получения перевода сообщения.
        /// </summary>
        private readonly IRoleMenuBuilderProvider _builderProvider;

        /// <summary>
        /// Создаёт новый экземпляр <see cref="MenuRoleAdapter"/>.
        /// </summary>
        /// <param name="userService">Сервис для работы с пользователями.</param>
        /// <param name="localizationService">Сервис локализации текста.</param>
        public MenuRoleAdapter(IRoleMenuBuilderProvider builderProvider)
        {
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
            if(string.IsNullOrWhiteSpace(parameter))
            {
                return new MessageResult { Text = "Параметр не может быть пустым." };
            }
            var builder = _builderProvider.GetBuilder(parameter);
            if (builder == null)
            {
                return new MessageResult { Text = "Неизвестный шаг меню." };
            }
            return await builder.BuildMenu(chatId);
        }
    }
}
