using FloristAI.Application.Store;
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

        public GoogleSheetsService(IGoogleSheets googleSheets)
        {
            _googleSheets = googleSheets;
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
            // Логика:
            // 1. Найти ID таблицы по userId (из БД).
            // 2. Вызвать Google Sheets API для чтения данных.
            // 3. Посчитать сумму за текущий месяц.
            return 0m; // пока заглушка
        }

        public async Task<string> GetGoogleSheetsUrl(int userId)
        {
            // Логика:
            // 1. Найти ID таблицы по userId.
            // 2. Сформировать URL: $"https://docs.google.com/spreadsheets/d/{spreadsheetId}"
            return "https://docs.google.com/spreadsheets/d/...";
        }
    }

}
