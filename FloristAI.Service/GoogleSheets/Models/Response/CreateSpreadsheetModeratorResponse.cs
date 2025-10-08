using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.GoogleSheets.Models.Response
{
    public class CreateSpreadsheetModeratorResponse
    {
        public string SheetName { get; set; } = string.Empty;
        public string SpreadsheetId { get; set; } = string.Empty;
    }
}
