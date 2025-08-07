using FloristAI.Application.GoogleSheets.Models.Request;
using FloristAI.Application.Store.Models.Response;


namespace FloristAI.Application.Store
{
    public interface IGoogleSheets
    {
        Task<CreateSpreadsheetResponse> CreateSpreadsheet(string name, string parentFolderId);
        Task<string>AddSheet(string spreadsheetId, string sheetName);
        Task AddHeaders(string spreadsheetId, string range, List<string[]> headers);
        Task AddData(AddDataRequest request);
        Task<IList<IList<object>>> GetValues(string spreadsheetId, string range);
    }
}
