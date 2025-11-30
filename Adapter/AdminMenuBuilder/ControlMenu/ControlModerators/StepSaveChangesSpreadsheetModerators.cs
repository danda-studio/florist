using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepMenuBuilder;
using FloristAI.Application.GoogleSheets;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.AdminMenuBuilder.ControlMenu.ControlModerators
{
    public class StepSaveChangesSpreadsheetModerators : IStepMenuBuilder
    {
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly IGoogleSheetsService _googleSheetsService;
        public StepSaveChangesSpreadsheetModerators(IUserService userService, ILocalizationService localizationService, IGoogleSheetsService googleSheetsService)
        {
            _userService = userService;
            _localizationService = localizationService;
            _googleSheetsService = googleSheetsService;
        }

        public string Step => "save_changes_moderators";

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
            try
            {
                var spreadsheet = await _googleSheetsService.GetModeratorSpreadsheet(_localizationService.GetSheetName("Moderator"));

                var dataTable = await _googleSheetsService.GetValues(spreadsheet.SpreadSheetId, "A:A");

                var userIds = dataTable
                    .Select(row => row.FirstOrDefault()?.ToString())
                    .Where(val => !string.IsNullOrEmpty(val) && long.TryParse(val, out _))
                    .Select(val => long.Parse(val!))
                    .ToList();

                var tasks = userIds.Select(async userId =>
                {
                    var user = await _userService.GetUser(userId);
                    return new { userId, Exists = user.UserId != 0 && user != null };
                }).ToList();

                var results = await Task.WhenAll(tasks);

                foreach (var result in results)
                {
                    if (result.Exists == false)
                    {
                        await _userService.GetOrCreateUser(result.userId, "ru", true);
                    }
                }
            }
            catch (Exception ex)
            {
                return new List<MessageResult>
                {
                    new MessageResult
                    {
                        Text = _localizationService.GetString("Control_Error_Importing", user.LanguageCode) + $"\n{ex.Message}",
                        ReplyMarkup = null
                    }
                };
            }

            var keyboard = new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Button_Menu", user.LanguageCode), "role_menu:Admin") }
            };
            return new List<MessageResult>
            {
                new MessageResult
                {
                    Text = _localizationService.GetString("Control_Moderator_Success", user.LanguageCode),
                    ReplyMarkup = keyboard
                }
            };
        }
    }
}
