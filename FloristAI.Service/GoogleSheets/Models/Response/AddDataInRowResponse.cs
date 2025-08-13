using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.GoogleSheets.Models.Response
{
    public class AddDataInRowResponse
    {
        public int UserId { get; set; }
        public string SpreadsheetId { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
