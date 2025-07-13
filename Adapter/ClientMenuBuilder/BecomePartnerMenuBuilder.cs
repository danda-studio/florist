using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepMenuBuilder;
using FloristAI.Application.Language;
using FloristAI.Application.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.ClientMenuBuilder
{
    public class BecomePartnerMenuBuilder: IStepMenuBuilder
    {
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        public BecomePartnerMenuBuilder(IUserService userService, ILocalizationService localizationService)
        {
            _userService = userService;
            _localizationService = localizationService;
        }
        
        public string Step => "become_partner";

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
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Become_Fill_Button", user.LanguageCode), "step:becomePartnerDescription") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Button_Menu", user.LanguageCode), "role_menu:Client") },
            };
            return new MessageResult
            {
                Text = _localizationService.GetString("Become_Partner_Menu", user.LanguageCode),
                ReplyMarkup = keyboard
            };
        }

    }
}
