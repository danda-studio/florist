using FloristAI.Adapter.Adapter.Models;
using FloristAI.Application.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Adapter.Adapter
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

            var role = await _userService.GetRolesByTelegramId(chatId);
            // parameter = "ru" или "ro"
            // например, сохраняем в DB выбранный язык
            return new MessageResult
            {
                Text = parameter == "ru"
                    ? "Язык изменён на русский."
                    : "Limba a fost schimbată în română."
            };
        }

    }
}
