using FloristAI.Application.GoogleSheets.Models.Request;
using FloristAI.Application.GoogleSheets.Models.Response;
using FloristAI.Application.Store.Models.Response;


namespace FloristAI.Application.GoogleSheets
{
    public interface IGoogleSheetsService
    {
        Task<CreateSpreadsheetResponse> CreateSpreadsheet(string name, string parentFolderId);
        Task<List<CreateStructureSheetResponse>> CreateStructureSheet(SheetsCreationParams parameters);
        Task AddSheet(string spreadsheetId, string sheetName);
        Task<decimal> GetMonthlyIncome(int userId);
        Task<string> GetGoogleSheetsUrl(int userId);
        Task<AddDataInRowResponse> AddDataInRow(AddDataRequest request);

        Task DeleteDefaultSheet(string spreadsheetId);
    }
}
