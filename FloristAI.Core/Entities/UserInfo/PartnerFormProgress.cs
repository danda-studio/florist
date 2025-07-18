using FloristAI.Core.Entities.Enums;

namespace FloristAI.Core.Entities.UserInfo
{
    public class PartnerFormProgress
    {
        public long ChatId { get; set; }           
        public string? Step { get; set; }   
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
    }

}
