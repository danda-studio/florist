using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.Users.Models.Response
{
    public class UpdatePartnerOnActivationModel
    {
        public int? UserId { get; set; }
        public string SpreadsheetId { get; set; } = string.Empty;
        public string PrivateSpreadsheetId { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string InviteCode { get; set; } = string.Empty;
    }
}
