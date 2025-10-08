using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Infrastructure.Models.Response
{
    public class CreateUserWithChatDataResponse
    {
        public long ChatId { get; set; }
        public string LanguageCode { get; set; } = "ru";
        public bool IsModerator { get; set; } 
    }
}
