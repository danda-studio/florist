using FloristAI.Application.GoogleSheets.Models.Request;
using FloristAI.Application.Store.Models.Response;
using Google.Apis.Sheets.v4.Data;


namespace FloristAI.Application.Store
{
    public interface IGoogleSheets
    {
        Task<IList<IList<object>>> GetValues(string spreadsheetId, string range);
        Task<IList<Sheet>> GetSheets(string spreadsheetId);
        Task<string> AddSheet(string spreadsheetId, string sheetName);
        Task AddHeaders(string spreadsheetId, string range, List<string[]> headers);
        Task AddData(AddDataRequest request);
        Task<CreateSpreadsheetResponse> CreateSpreadsheet(string name, string parentFolderId);
        Task UpdateValue(string spreadsheetId, string range, string value);
        Task DeleteDefaultSheet(string spreadsheetId);
        Task<(bool Success, Google.Apis.Drive.v3.Data.File? File)> FindSpreadsheet(string name, string? parentFolderId = null);

    }
}
