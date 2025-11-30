using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepMenuBuilder;
using FloristAI.Application.GoogleSheets;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.AdminMenuBuilder.ControlMenu.ControlBoutiques
{
    public class ControlBoutiques : IStepMenuBuilder
    {
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly IGoogleSheetsService _googleSheetsService;

        public ControlBoutiques(IUserService userService, ILocalizationService localizationService, IGoogleSheetsService googleSheetsService)
        {
            _userService = userService;
            _localizationService = localizationService;
            _googleSheetsService = googleSheetsService;
        }

        public string Step => "control_boutiques_admin";

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
            var spreadsheet = await _googleSheetsService.GetBoutiqueSpreadsheet(_localizationService.GetSheetName("Boutique"));
            var googleSheetsUrl = $"https://docs.google.com/spreadsheets/d/{spreadsheet.SpreadSheetId}";
            
            var header = _localizationService.GetString("Control_Header_Boutiques", user.LanguageCode);
            var body = _localizationService.GetString("Control_Instructions", user.LanguageCode);
            var messageText = string.Format(header + "\n" + body, $"<a href=\"{googleSheetsUrl}\">{_localizationService.GetString("Text_Table", user.LanguageCode)}</a>");
            
            var keyboard = new[]
            {
                new[] { InlineKeyboardButton.WithUrl(_localizationService.GetString("Button_Go_To_Table", user.LanguageCode), googleSheetsUrl) },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Button_Save_Changes", user.LanguageCode), "step:save_changes_boutiques_admin") },
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Button_Menu", user.LanguageCode), "role_menu:Admin") },
            };
            
            return new List<MessageResult>
            {
                new MessageResult
                {
                    Text = messageText,
                    ReplyMarkup = keyboard
                }
            };
        }
    }
}
