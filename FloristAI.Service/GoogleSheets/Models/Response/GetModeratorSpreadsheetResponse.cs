
using File = Google.Apis.Drive.v3.Data.File;

namespace FloristAI.Application.GoogleSheets.Models.Response
{
    public class GetModeratorSpreadsheetResponse
    {
        public bool Success { get; set; }
        public string SpreadSheetId { get; set; } = string.Empty;
    }
}
