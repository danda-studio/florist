using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.GoogleSheets.Models.Request
{
    public class CreateStructureSheetRequest
    {
        public int PartnerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PublicFolderId { get; set; }
        public string? PrivateFolderId { get; set; }
    }
}
