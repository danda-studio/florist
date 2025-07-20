using FloristAI.Core.Entities.Enums;
using System;

namespace FloristAI.Application.Users.Models.Response
{
    public class GetStepResponse
    {
        public long ChatId { get; set; }  
        public string? Step { get; set; }   // Текущий шаг
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
    }
}
