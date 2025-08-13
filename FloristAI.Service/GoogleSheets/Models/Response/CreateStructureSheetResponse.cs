using FloristAI.Application.GoogleSheets.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.GoogleSheets.Models.Response
{
    public class CreateStructureSheetResponse
    {
        public string SpreadsheetId { get; set; } = string.Empty;
        public string SheetName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public bool IsPublic { get; set; } = false;
    }
}
