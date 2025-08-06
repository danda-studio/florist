using FloristAI.Application.GoogleSheets.Models.Request;
using FloristAI.Application.Store.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.GoogleSheets
{
    public interface IGoogleSheetsService
    {
        Task<CreateSpreadsheetResponse> CreateSpreadsheet(string name, string parentFolderId);
        Task<string> CreateStructureSheet(SheetsCreationParams parameters);
        Task AddSheet(string spreadsheetId, string sheetName);
        Task<decimal> GetMonthlyIncome(int userId);
        Task<string> GetGoogleSheetsUrl(int userId);
    }
}
