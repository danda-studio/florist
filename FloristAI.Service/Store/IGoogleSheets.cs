using FloristAI.Application.Store.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.Store
{
    public interface IGoogleSheets
    {
        Task<CreateSpreadsheetResponse> CreateSpreadsheet(string name, string parentFolderId);
        Task<string>AddSheet(string spreadsheetId, string sheetName);
        Task AddHeaders(string spreadsheetId, string range, List<string[]> headers);
        Task<IList<IList<object>>> GetValues(string spreadsheetId, string range);
    }
}
