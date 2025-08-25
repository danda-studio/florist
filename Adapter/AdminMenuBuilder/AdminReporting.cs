using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepMenuBuilder;
using FloristAI.Application.GoogleSheets;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.AdminMenuBuilder
{
    public class AdminReporting : IStepMenuBuilder
    {
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly IGoogleSheetsService _googleSheetsService;
        public AdminReporting(IUserService userService, ILocalizationService localizationService, IGoogleSheetsService googleSheetsService)
        {
            _userService = userService;
            _localizationService = localizationService;
            _googleSheetsService = googleSheetsService;
        }

        public string Step => "reporting_bussines";

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

            var googleSheetsUrl = await _googleSheetsService.GetAdminGoogleSheetsUrl();
            var keyboard = new[]
            {
                new[] { InlineKeyboardButton.WithUrl(_localizationService.GetString("Button_Go_To_Table", user.LanguageCode), googleSheetsUrl)},
                new[] { InlineKeyboardButton.WithUrl(_localizationService.GetString("Button_Turnover", user.LanguageCode), googleSheetsUrl) },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Button_Menu", user.LanguageCode), "role_menu:Admin") },

            };
            return new List<MessageResult>
            {
                new MessageResult
                {
                    Text = _localizationService.GetString("Menu_Reporting_Admin", user.LanguageCode),
                    ReplyMarkup = keyboard
                }
            };
        }
    }
}
