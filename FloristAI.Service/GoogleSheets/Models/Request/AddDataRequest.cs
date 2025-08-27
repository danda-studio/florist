using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.GoogleSheets.Models.Request
{
    public class AddDataRequest
    {
        public int UserId { get; set; }

        public List<SheetInfo> Sheets { get; set; } = new List<SheetInfo>();
        public string SpreadsheetId { get; set; } = string.Empty;
        public string PrivateSpreadsheetId { get; set; } = string.Empty;
        public string SheetName { get; set; } = string.Empty;

        public UserData UserData { get; set; } = new UserData();
        
    }
}
