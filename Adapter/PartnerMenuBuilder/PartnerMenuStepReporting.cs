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

namespace FloristAI.Adapter.PartnerMenuBuilder
{
    public class PartnerMenuStepReporting : IStepMenuBuilder
    {

        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;

        public PartnerMenuStepReporting(IUserService userService, ILocalizationService localizationService)
        {
            _userService = userService;
            _localizationService = localizationService;
        }
        public string Step => "reporting";


        public async Task<List<MessageResult>> BuildMenu(long chatId)
        {
            var user = await _userService.GetUser(chatId);
            if (user == null)
            {
                return new List<MessageResult>
                {
                    new MessageResult
                    {
                        Text = _localizationService.GetString("UserNotFound", "ru"),
                        ReplyMarkup = null
                    }
                };
            }
            var keyboard = new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Button_Go_To_Table", user.LanguageCode), "step:referal_url") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Update_Data", user.LanguageCode), "step:reporting") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Button_Menu", user.LanguageCode), "role_menu:Partner") },

            };
            return new List<MessageResult>
            {
                new MessageResult
                {
                    Text = _localizationService.GetString("Menu_Partner", user.LanguageCode),
                    ReplyMarkup = keyboard
                }
            };

        }
    }
}
