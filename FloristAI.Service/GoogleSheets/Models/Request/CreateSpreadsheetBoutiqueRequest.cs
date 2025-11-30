using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.GoogleSheets.Models.Request
{
    public class CreateSpreadsheetBoutiqueRequest
    {
        public string FolderId { get; set; } = string.Empty;

        public string SheetName { get; set; } = string.Empty;
    }
}
