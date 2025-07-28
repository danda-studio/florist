using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.Store
{
    public interface IGoogleSheets
    {
        Task<string> CreateSpreadsheet(string name, string parentFolderId);
        Task AddSheet(string spreadsheetId, string sheetName);
    }
}
