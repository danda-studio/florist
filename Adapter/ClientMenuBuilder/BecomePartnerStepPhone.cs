using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepMenuBuilder;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.ClientMenuBuilder
{
    public class BecomePartnerStepPhone : IStepMenuBuilder
    {
        private readonly IUserService _userService;

        private readonly ILocalizationService _localizationService;
        public BecomePartnerStepPhone(IUserService userService, ILocalizationService localizationService)
        {
            _userService = userService;
            _localizationService = localizationService;
        }

        public string Step => "become_partner_step_phone";

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
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Button_Back", user.LanguageCode), "step:become_partner_step_lastName") },
            };
            return new MessageResult
            {
                Text = _localizationService.GetString("Become_Input_Phone", user.LanguageCode),
                ReplyMarkup = keyboard
            };
        }
    }
}
