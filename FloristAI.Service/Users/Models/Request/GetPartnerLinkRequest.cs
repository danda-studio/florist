using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.Users.Models.Request
{
    public class GetPartnerLinkRequest
    {
        public long ChatId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string InviteCode { get; set; } = string.Empty;
        public bool IsActive { get; set; } 
    }
}
