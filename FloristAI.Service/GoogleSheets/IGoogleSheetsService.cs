using FloristAI.Application.GoogleSheets.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.GoogleSheets
{
    public interface IGoogleSheetsService
    {
        Task<string> CreateSpreadsheet(string name, string parentFolderId);
        Task<string> CreateStructureSheet(SheetsCreationParams parameters);
        Task AddSheet(string spreadsheetId, string sheetName);
        Task<decimal> GetMonthlyIncome(int userId);
        Task<string> GetGoogleSheetsUrl(int userId);
    }
}
