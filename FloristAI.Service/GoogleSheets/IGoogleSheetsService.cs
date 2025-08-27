using FloristAI.Application.GoogleSheets.Models.Request;
using FloristAI.Application.GoogleSheets.Models.Response;
using FloristAI.Application.Store.Models.Response;


namespace FloristAI.Application.GoogleSheets
{
    public interface IGoogleSheetsService
    {
        Task<string> GetSheetIdByMonth(string spreadsheetId, DateTime date);
        Task<IList<IList<object>>> GetValues(string spreadsheetId, string range);
        Task<decimal> GetMonthlyIncome(int userId);
        Task<string> GetGoogleSheetsUrl(int userId);
        Task<string> GetAdminGoogleSheetsUrl();
        Task<CreateSpreadsheetResponse> CreateSpreadsheet(string name, string parentFolderId);
        Task<List<CreateStructureSheetResponse>> CreateStructureSheet(SheetsCreationParams parameters);
        Task AddSheet(string spreadsheetId, string sheetName);
        Task<AddDataInRowResponse> AddDataInRow(AddDataRequest request);
        Task UpdateValue(string spreadsheetId, string range, string value);
        Task DeleteDefaultSheet(string spreadsheetId);
        Task<string?> FindSpreadsheet(string name);
    }
}
