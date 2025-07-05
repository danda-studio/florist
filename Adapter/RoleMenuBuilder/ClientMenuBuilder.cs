using FloristAI.Adapter.Models;
using FloristAI.Application.Language;
using FloristAI.Application.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.RoleMenuBuilder
{
    public class ClientMenuBuilder : IRoleMenuBuilder
    {
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        public ClientMenuBuilder(IUserService userService, ILocalizationService localizationService)
        {
            _userService = userService;
            _localizationService = localizationService;
        }
        public string Role => "Client";

        public async Task<MessageResult> BuildMenu(long chatId)
        {
            var user = await _userService.GetUser(chatId);
            if (user == null)
            {
                return new MessageResult
                {
                    Text = _localizationService.GetString("UserNotFound", user.LanguageCode),
                    ReplyMarkup = null
                };
            }
            var keyboard = new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Flower", user.LanguageCode), "get_menu:flower") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Basket", user.LanguageCode), "get_menu:basket") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Bouquet", user.LanguageCode), "get_menu:bouquet") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Create_Bouquet", user.LanguageCode), "get_menu:createBouquet") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("My_Orders", user.LanguageCode), "get_menu:myOrder") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Become_Partner", user.LanguageCode), "get_menu:becomePartner") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("LanguageSelection", user.LanguageCode), "start") },
            };
            return new MessageResult
            {
                Text = _localizationService.GetString("Menu_Client", user.LanguageCode),
                ReplyMarkup = keyboard
            };
        }

    }
}
