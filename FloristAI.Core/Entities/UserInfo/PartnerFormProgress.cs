using FloristAI.Core.Entities.Enums;

namespace FloristAI.Core.Entities.UserInfo
{
    public class PartnerFormProgress
    {
        public long ChatId { get; set; }           
        public string Step { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

}
