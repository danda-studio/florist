using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.User.Models.Response
{
    public class GetOrCreateUserResponse
    {
        public int Id { get; set; }
        public string? LanguageCode { get; set; }
    }
}
