using Google.Apis.Drive.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Infrastructure.Models.Response
{
    public class FindSpreadsheetResponse
    {
        public bool Success { get; set; }
        public Google.Apis.Drive.v3.Data.File? File { get; set; } 
    }
}
