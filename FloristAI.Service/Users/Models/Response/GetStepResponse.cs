using FloristAI.Core.Entities.Enums;
using System;

namespace FloristAI.Application.Users.Models.Response
{
    public class GetStepResponse
    {
        public long ChatId { get; set; }  
        public string Step { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
