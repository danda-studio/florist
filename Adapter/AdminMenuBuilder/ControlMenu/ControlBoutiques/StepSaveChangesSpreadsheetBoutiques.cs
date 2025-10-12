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

namespace FloristAI.Adapter.AdminMenuBuilder.ControlMenu.ControlBoutiques
{
    public class StepSaveChangesSpreadsheetBoutiques : IStepMenuBuilder
    {
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;
        private readonly IGoogleSheetsService _googleSheetsService;
        public StepSaveChangesSpreadsheetBoutiques(IUserService userService, ILocalizationService localizationService, IGoogleSheetsService googleSheetsService)
        {
            _userService = userService;
            _localizationService = localizationService;
            _googleSheetsService = googleSheetsService;
        }

        public string Step => "save_changes_boutiques";

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
                var spreadsheet = await _googleSheetsService.GetBoutiqueSpreadsheet(_localizationService.GetSheetName("Boutique"));

                //var dataTable = await _googleSheetsService.GetValues(spreadsheet.SpreadSheetId, "A2:C"); 
                //var shops = dataTable 
                //    .Where(row => row.Count >= 2) // Должен быть хотя бы Адрес
                //    .Select(row => new Shop 
                //    { 
                //        Address = row[1], // Колонка B
                //        UrlGoogleMap = row.Count >= 3 ? row[2] : "", // Колонка C
                //    })
                //    .ToList(); 
                
                //// Теперь можно сохранить в базу
                //foreach (var shop in shops) 
                //{ 
                //    await _shopService.CreateOrUpdateShop(shop); 
                //}
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
                    Text = _localizationService.GetString("Control_Boutique_Success", user.LanguageCode),
                    ReplyMarkup = keyboard
                }
            };
        }
    }
}
