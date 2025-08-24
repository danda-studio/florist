using FloristAI.Adapter.Models;
using FloristAI.Adapter.RoleMenuBuilder;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.ClientMenuBuilder
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
                    Text = _localizationService.GetString("UserNotFound", "ru"),
                    ReplyMarkup = null
                };
            }
            var keyboard = new List<InlineKeyboardButton[]>
            {
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Flower", user.LanguageCode), "step:flower") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Basket", user.LanguageCode), "step:basket") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Bouquet", user.LanguageCode), "step:bouquet") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Create_Bouquet", user.LanguageCode), "step:create_bouquet") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("My_Orders", user.LanguageCode), "step:my_order") }
            };

            if (!user.IsPartner)
            {
                keyboard.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        _localizationService.GetString("Become_Partner", user.LanguageCode),
                        "step:become_partner")
                });
            }

            if (user.Roles.Count > 1)
            {
                keyboard.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        _localizationService.GetString("Select_Room", user.LanguageCode),
                        $"select_role:{user.LanguageCode}")
                });
            }
            else
            {
                keyboard.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        _localizationService.GetString("LanguageSelection", user.LanguageCode),
                        "/start")
                });
            }
            return new MessageResult
            {
                Text = _localizationService.GetString("Menu_Client", user.LanguageCode),
                ReplyMarkup = new InlineKeyboardMarkup(keyboard),
                RemovePinnedMessage = true
            };

            
        }

    }
}
