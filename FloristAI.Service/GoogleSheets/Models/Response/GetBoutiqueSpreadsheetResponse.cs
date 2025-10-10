using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.GoogleSheets.Models.Response
{
    public class GetBoutiqueSpreadsheetResponse
    {
        public bool Success { get; set; }
        public string SpreadSheetId { get; set; } = string.Empty;
    }
}
