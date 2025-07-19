using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepFlowBuilder;
using FloristAI.Adapter.StepMenuBuilder;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using FloristAI.Application.Users.Models.Request;
using FloristAI.Core.Entities.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.ClientMenuBuilder.BecomePartnerStep
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
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Become_Fill_Button", user.LanguageCode), "step_message:become_partner_step_firstName") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Button_Menu", user.LanguageCode), "role_menu:Client") },
            };
            return new MessageResult
            {
                Text = _localizationService.GetString("Become_Partner_Title", user.LanguageCode),
                ReplyMarkup = keyboard
            };
        }

    }
}
