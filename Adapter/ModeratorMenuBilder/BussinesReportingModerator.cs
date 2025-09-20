using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepMenuBuilder;
using FloristAI.Application.GoogleSheets;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.ModeratorMenuBilder
{
    public class BussinesReportingModerator : IStepMenuBuilder
    {
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly IGoogleSheetsService _googleSheetsService;
        public BussinesReportingModerator(IUserService userService, ILocalizationService localizationService, IGoogleSheetsService googleSheetsService)
        {
            _userService = userService;
            _localizationService = localizationService;
            _googleSheetsService = googleSheetsService;
        }

        public string Step => "reporting_bussines_moderator";

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
                new[] { InlineKeyboardButton.WithUrl(_localizationService.GetString("Button_Partners", user.LanguageCode), googleSheetsUrl)},
                new[] { InlineKeyboardButton.WithUrl(_localizationService.GetString("Button_Turnover", user.LanguageCode), googleSheetsUrl) },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Button_Menu", user.LanguageCode), "role_menu:Moderator") },

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
