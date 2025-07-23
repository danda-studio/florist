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

        public async Task<List<MessageResult>> BuildMenu(long chatId)
        {
            var user = await _userService.GetUser(chatId);
            var isPartner = await _userService.CheckStatusPartner(chatId);

            var messages = new List<MessageResult>();

            if (isPartner)
            {
                var backButton = new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Button_Menu", user.LanguageCode), "role_menu:Client") }
                };

                messages.Add(new MessageResult
                {
                    Text = _localizationService.GetString("IsPartner", user.LanguageCode),
                    ReplyMarkup = backButton
                });

                return messages;
            }

            if (user == null)
            {
                messages.Add(new MessageResult
                {
                    Text = _localizationService.GetString("UserNotFound", "ru"),
                    ReplyMarkup = null
                });
                return messages;
            }

            messages.Add(new MessageResult
            {
                Text = _localizationService.GetString("Become_Partner_Description", user.LanguageCode),
                ReplyMarkup = null
            });

            var keyboard = new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Become_Fill_Button", user.LanguageCode), "step_message:become_partner_step_firstName") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Button_Menu", user.LanguageCode), "role_menu:Client") },
            };

            messages.Add(new MessageResult
            {
                Text = _localizationService.GetString("Become_Partner_Title", user.LanguageCode),
                ReplyMarkup = keyboard,
                PinnedMessage = true
            });

            return messages;
        }


    }
}
