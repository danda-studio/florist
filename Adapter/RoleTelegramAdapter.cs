using FloristAI.Adapter.Models;
using FloristAI.Application.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter
{
    public class RoleTelegramAdapter : IMessageAdapter
    {
        public string RouteKey => "select_language";

        private readonly IUserService _userService;

        public RoleTelegramAdapter(IUserService userService)
        {
            _userService = userService;
        }


        public async Task<MessageResult> ProcessMessage(string parameter, long chatId)
        {
            var langCode = await _userService.EditLanguageInterfaceUser(chatId, parameter);
            var role = await _userService.GetRolesByTelegramId(chatId, parameter);
            var buttons = role.Roles.Select(r => new [] 
            {
                InlineKeyboardButton.WithCallbackData(
                    text: r.RoleName,
                    callbackData:$"role_select:{r.RoleType}"
                )
            }).ToArray();

            return new MessageResult
            {
                Text = role.Roles.Count > 1
                    ? (parameter == "ru" ? "Вам доступно несколько ролей:" : "Aveți acces la mai multe roluri:")
                    : (parameter == "ru" ? "Ваша роль:" : "Rolul dvs.:"),
                ReplyMarkup = new InlineKeyboardMarkup(buttons)
            };
        }

    }
}
