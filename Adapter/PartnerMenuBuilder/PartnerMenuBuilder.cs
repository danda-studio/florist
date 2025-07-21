using FloristAI.Adapter.Models;
using FloristAI.Adapter.RoleMenuBuilder;
using FloristAI.Application.Language;
using FloristAI.Application.Users;

using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.PartnerMenuBuilder
{
    public class PartnerMenuBuilder : IRoleMenuBuilder
    {
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        public PartnerMenuBuilder(IUserService userService, ILocalizationService localizationService)
        {
            _userService = userService;
            _localizationService = localizationService;
        }
        public string Role => "Partner";

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
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Referral_Link", user.LanguageCode), "step:referal_url") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Reporting", user.LanguageCode), "step:result") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Select_Room", user.LanguageCode), $"select_role:{user.LanguageCode}") },

            };
            return new MessageResult
            {
                Text = _localizationService.GetString("Menu_Partner", user.LanguageCode),
                ReplyMarkup = keyboard
            };


        }
    }
}
