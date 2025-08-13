using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.GoogleDrive.Models.Request
{
    public class CreateStructureFolderRequest
    {
        public int PartnerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
