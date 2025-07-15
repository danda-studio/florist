using FloristAI.Core.Entities.Enums;

namespace FloristAI.Application.Users.Models.Request
{
    public class SaveStepRequest
    {
        public long ChatId { get; set; }
        public CountStep Step { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }

    }
}
