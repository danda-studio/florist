using FloristAI.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Core.Entities.UserInfo
{
    public class PartnerFormProgress
    {
        public long ChatId { get; set; }           
        public PartnerFormStep Step { get; set; }   
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
    }

}
