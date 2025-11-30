using FloristAI.Adapter.Models;
using FloristAI.Adapter.RoleMenuBuilder;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using Telegram.Bot.Types.ReplyMarkups;


namespace FloristAI.Adapter.ModeratorMenuBilder
{
    public class ModeratorMenuBuilder : IRoleMenuBuilder
    {
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        public static readonly HashSet<long> _hasTempPin = new();
        public ModeratorMenuBuilder(IUserService userService, ILocalizationService localizationService)
        {
            _userService = userService;
            _localizationService = localizationService;
        }
        public string Role => "Moderator";

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
            var keyboard = new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Control_Flowers", user.LanguageCode), "step:control_flowers_moder") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Control_Bouquets", user.LanguageCode), "step:control_bouquets") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Control_Baskets", user.LanguageCode), "step:control_baskets") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Generate_PartnerLink", user.LanguageCode), "step_message:generate_partnerlink_firstname_moderator") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Send_Messages", user.LanguageCode), "step:sends_message") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Reporting", user.LanguageCode), "step:reporting_bussines_moderator") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Control_Shops", user.LanguageCode), "step:control_boutiques_moder") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Select_Room", user.LanguageCode), $"select_role:{user.LanguageCode}") }
            };

            bool removePin = _hasTempPin.Contains(chatId);
            if (removePin)
                _hasTempPin.Remove(chatId);

            return new MessageResult
            {
                Text = _localizationService.GetString("Menu_Moderator", user.LanguageCode),
                ReplyMarkup = keyboard,
                RemovePinnedMessage = removePin,
            };
        }
    }
}
