using FloristAI.Application.GoogleSheets.Models.Request;
using FloristAI.Application.Store;
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
                 IsNew: true),
                (name: $"{parameters.PartnerId}/{parameters.FirstName} {parameters.LastName}/{DateTime.Now.Year}",
                 folderId: parameters.PrivateFolderId,
                 IsPublic: false,
                 IsNew: true),
                (name: "Общая информация",
                 folderId: parameters.PrivateFolderId,
                 IsPublic: false,
                 IsNew: true)
            };

            string publicSpreadsheetId = "";
            var monthSheetName = DateTime.Now.ToString("MMMM");

            foreach (var spreadsheet in spreadsheetsToCreate)
            {
                // Проверяем, существует ли уже таблица
                var spreadsheetId = await _googleSheets.CreateSpreadsheet(
                    spreadsheet.name,
                    spreadsheet.folderId);

                await _googleSheets.AddSheet(spreadsheetId, monthSheetName);

                if (spreadsheet.IsNew)
                {
                    await _googleSheets.AddSheet(spreadsheetId, "Итог");
                }

                if (spreadsheet.IsPublic)
                {
                    publicSpreadsheetId = spreadsheetId;
                }
            }

            return publicSpreadsheetId ?? throw new InvalidOperationException("Не удалось создать публичную таблицу");
        }


        public async Task<string> CreateSpreadsheet(string name, string parentFolderId)
        {
            return await _googleSheets.CreateSpreadsheet(name, parentFolderId);
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
                throw new InvalidOperationException("Spreadsheet ID not found for user");

            return $"https://docs.google.com/spreadsheets/d/{spreadsheetId}";
        }
    }

}
