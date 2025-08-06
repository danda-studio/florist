using FloristAI.Application.GoogleSheets.Models.Request;
using FloristAI.Application.Store;
using FloristAI.Application.Store.Models.Response;
using FloristAI.Core.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.GoogleSheets
{
    public class GoogleSheetsService : IGoogleSheetsService
    {
        private readonly IGoogleSheets _googleSheets;
        private readonly IUserRepository _userRepository;

        public GoogleSheetsService(IGoogleSheets googleSheets, IUserRepository userRepository)
        {
            _googleSheets = googleSheets;
            _userRepository = userRepository;
        }

        public async Task<string> CreateStructureSheet(SheetsCreationParams parameters)
        {
            // Создаём все таблицы, но сохраняем ID публичной
            var spreadsheetsToCreate = new[]
            {
                (name: $"{parameters.PartnerId}/{parameters.FirstName} {parameters.LastName}/{DateTime.Now.Year}",
                 folderId: parameters.PublicFolderId,
                 IsPublic: true,
                 FlagName: "PublicPartnerInfo"),
                (name: $"{parameters.PartnerId}/{parameters.FirstName} {parameters.LastName}/{DateTime.Now.Year}",
                 folderId: parameters.PrivateFolderId,
                 IsPublic: false,
                 FlagName: "PrivatePartnerInfo"),
                (name: "Общая информация",
                 folderId: parameters.PrivateFolderId,
                 IsPublic: false,
                 FlagName: "TotalInfo")
            };

            var headersConfig = new Dictionary<string, List<string[]>>
            {
                {
                    "PublicPartnerInfo", new List<string[]>
                    {
                        new[] {"Итого"},
                        new[] {"Дата", "Доход" }
                    }
                },
                {
                    "PrivatePartnerInfo", new List<string[]>
                    {
                        new[] {"Итого"},
                        new[] {"Дата", "Доход партнера(5%)", "Доход без комисси партнера(95%)", "Общий доход(100%)" }
                    }
                },
                {
                    "TotalInfo", new List<string[]>
                    {

                        new[] {"Итого"},
                        new[] {"ID", "Фамилия,Имя", "Номер телефона", "Telegram ID", "Telegran USERNAME", "Кол-во рефералов", "Общий доход(5%)", "Общий доход,без комисии партнера (95%)", "Общий доход(100%)" }
                    }
                }
            };

            string publicSpreadsheetId = "";
            var monthSheetName = DateTime.Now.ToString("MMMM");

            foreach (var spreadsheet in spreadsheetsToCreate)
            {
                // Проверяем, существует ли уже таблица
                var sheetInfo = await CreateSpreadsheet(spreadsheet.name, spreadsheet.folderId);
                if (sheetInfo.IsNew == false)
                    return sheetInfo.SpreadsheetId;

                await _googleSheets.AddSheet(sheetInfo.SpreadsheetId, monthSheetName);


                if (headersConfig.TryGetValue(spreadsheet.FlagName, out var headers))
                {
                    int maxColumns = headers.Max(h => h.Length);
                    int rows = headers.Count;

                    var lastColumn = GetColumnLetter(maxColumns);
                    var range = $"{monthSheetName}!A1:{lastColumn}{rows}";

                    await _googleSheets.AddHeaders(sheetInfo.SpreadsheetId, range, headers);
                }


                if (sheetInfo.IsNew)
                {
                    await _googleSheets.AddSheet(sheetInfo.SpreadsheetId, "Итог");
                }

                if (spreadsheet.IsPublic)
                {
                    publicSpreadsheetId = sheetInfo.SpreadsheetId;
                }
            }

            return publicSpreadsheetId ?? throw new InvalidOperationException("Не удалось создать публичную таблицу");
        }


        public async Task<CreateSpreadsheetResponse> CreateSpreadsheet(string name, string parentFolderId)
        {

            var sheet = await _googleSheets.CreateSpreadsheet(name, parentFolderId);
            return new CreateSpreadsheetResponse
            {
                SpreadsheetId = sheet.SpreadsheetId,
                IsNew = sheet.IsNew
            };
        }

        public async Task AddSheet(string spreadSheetId, string sheetName)
        {
            await _googleSheets.AddSheet(spreadSheetId, sheetName);
        }

        public async Task<decimal> GetMonthlyIncome(int userId)
        {

            var spreadsheetId = await _userRepository.GetSpreadsheetId(userId);
            if (string.IsNullOrEmpty(spreadsheetId))
                throw new InvalidOperationException("Spreadsheet ID not found for user");

            var values = await _googleSheets.GetValues(spreadsheetId, "A:B");

            if (values.Count == 0)
                return 0m;

            decimal total = 0m;
            var currentMonth = DateTime.Now.Month;

            foreach (var row in values.Skip(1))
            {
                if (row.Count < 2) continue;

                if (DateTime.TryParse(row[0]?.ToString(), out var date) &&
                    decimal.TryParse(row[1]?.ToString(), out var amount))
                {
                    if (date.Month == currentMonth)
                        total += amount;
                }
            }

            return total;
        }


        public async Task<string> GetGoogleSheetsUrl(int userId)
        {
            var spreadsheetId = await _userRepository.GetSpreadsheetId(userId);
            if (string.IsNullOrEmpty(spreadsheetId))
                spreadsheetId = "0";

            return $"https://docs.google.com/spreadsheets/d/{spreadsheetId}";
        }


        private static string GetColumnLetter(int columnIndex)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var result = string.Empty;

            while (columnIndex > 0)
            {
                columnIndex--;
                result = letters[columnIndex % 26] + result;
                columnIndex /= 26;
            }

            return result;
        }
    }

}
