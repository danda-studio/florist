using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.Users.Models.Request
{
    public class ProcessReferralRequest
    {
        public int UserId { get; set; }
        public int PartnerId { get; set; }
        public string LanguageCode { get; set; } = "ru";
    }
}
