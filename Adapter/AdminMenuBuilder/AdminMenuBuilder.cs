using FloristAI.Adapter.Models;
using FloristAI.Adapter.RoleMenuBuilder;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.AdminMenuBuilder
{
    public class AdminMenuBuilder : IRoleMenuBuilder
    {
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        public AdminMenuBuilder(IUserService userService, ILocalizationService localizationService)
        {
            _userService = userService;
            _localizationService = localizationService;
        }
        public string Role => "Admin";

        public async Task<MessageResult> BuildMenu(long chatId)
        {
            var user = await _userService.GetUser(chatId);
            if (user == null)
            {
                return new MessageResult
                {
                    Text = _localizationService.GetString("UserNotFound", "ru"),
                    ReplyMarkup = null
                };
            }
            var keyboard = new []
            {
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Control_Flowers", user.LanguageCode), "step:control_flowers") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Control_Bouquets", user.LanguageCode), "step:control_bouquets") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Control_Baskets", user.LanguageCode), "step:control_baskets") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Generate_PartnerLink", user.LanguageCode), "step_message:generate_partnerlink_firstname") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Send_Messages", user.LanguageCode), "step:sends_message") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Reporting", user.LanguageCode), "step:reporting_bussines") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Control_Shops", user.LanguageCode), "step:control_shop") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Control_Moderators", user.LanguageCode), "step:control_moderators") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Select_Room", user.LanguageCode), $"select_role:{user.LanguageCode}") }
            };
            return new MessageResult
            {
                Text = _localizationService.GetString("Menu_Admin", user.LanguageCode),
                ReplyMarkup = keyboard,
                RemovePinnedMessage = true,
            };
        }
    }
}
