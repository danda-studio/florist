using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepMenuBuilder;
using FloristAI.Application.Boutique;
using FloristAI.Application.Boutique.Model.Request;
using FloristAI.Application.GoogleSheets;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using FloristAI.Core.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.AdminMenuBuilder.ControlMenu.ControlBoutiques
{
    public class StepSaveChangesSpreadsheetBoutiques : IStepMenuBuilder
    {
        private readonly IUserService _userService;
        private readonly IShopService _shopService;
        private readonly ILocalizationService _localizationService;
        private readonly IGoogleSheetsService _googleSheetsService;
        public StepSaveChangesSpreadsheetBoutiques(IUserService userService, IShopService shopService, ILocalizationService localizationService, IGoogleSheetsService googleSheetsService)
        {
            _userService = userService;
            _shopService = shopService;
            _localizationService = localizationService;
            _googleSheetsService = googleSheetsService;
        }

        public string Step => "save_changes_boutiques_admin";

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

                var rows = await _googleSheetsService.GetValues(spreadsheet.SpreadSheetId, $"A2:B");
                
                var shops = new List<Shop>();
                
                var validationErrors = new List<string>();

                for (int i = 0; i < rows.Count; i++)
                {
                    var name = rows[i].Count > 0 ? rows[i][0]?.ToString()?.Trim() : null;
                    var address = rows[i].Count > 1 ? rows[i][1]?.ToString()?.Trim() : null;

                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address))
                        continue;

                    var validation = await _shopService.ValidationAddress(new ValidationAddressRequest
                    {
                        Address = address,
                        Name = name,
                        LanguageCode = user.LanguageCode,
                    });

                    if (validation.Errors.Count > 0)
                    {
                        validationErrors.AddRange(validation.Errors); 
                        continue;
                    }

                    var coords = await _shopService.GetCoordinatesFromAddress(address);
                    if (coords == null)
                        continue;

                    var shop = new Shop
                    {
                        Address = address,
                        Latitude = coords.Latitude,
                        Longitude = coords.Longitude,
                        UrlGoogleMap = $"{coords.Latitude},{coords.Longitude}"
                    };

                    shops.Add(shop);
                }
                if (shops.Count > 0)
                {
                    await _shopService.AddShops(new AddShopsRequest
                    {
                        Shops = shops.Select(s => new ShopModel
                        {
                            Address = s.Address,
                            Latitude = s.Latitude,
                            Longitude = s.Longitude,
                            UrlGoogleMap = s.UrlGoogleMap,
                            LanguageCode = user.LanguageCode
                        }).ToList()
                    });
                }

                if (validationErrors.Count > 0)
                {
                    var googleSheetsUrl = $"https://docs.google.com/spreadsheets/d/{spreadsheet.SpreadSheetId}";
  
                    return new List<MessageResult>
                    {
                        new MessageResult
                        {
                            Text = string.Join("\n", validationErrors),
                            ReplyMarkup = new[]
                            {
                                new[] { InlineKeyboardButton.WithUrl(_localizationService.GetString("Button_Go_To_Table", user.LanguageCode),googleSheetsUrl) },
                                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Button_Save_Changes", user.LanguageCode), "step:save_changes_boutiques_moder") },
                                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Button_Menu", user.LanguageCode), "role_menu:Moderator") }
                            }
                        }
                    };
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
                    Text = _localizationService.GetString("Control_Boutique_Success", user.LanguageCode),
                    ReplyMarkup = keyboard
                }
            };
        }
    }
}
