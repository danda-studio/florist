using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.Users.Models.Request
{
    public class UpdatePartnerOnActivationRequest
    {
        public long ChatId { get; set; }
        public string SpreadSheetId { get; set; } = string.Empty;
        public string InviteCode { get; set; } = string.Empty;
        public string PrivateSpreadSheetId { get; set; } = string.Empty;
    }
}
