using FloristAI.Core.Entities.UserInfo;

namespace FloristAI.Core.Entities.ReferralsAndPartners
{
    /// <summary>
    /// Модель партнёра, участвующего в реферальной программе.
    /// </summary>
    public class Partner
    {
        /// <summary>
        /// Уникальный идентификатор партнёра.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя, связанного с партнёром.
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Имя партнёра.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Фамилия партнёра.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Номер телефона партнёра.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Пользователь, которому принадлежит статус партнёра.
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Список рефералов, приглашённых данным партнёром.
        /// </summary>
        public List<PartnerReferal> Partners { get; set; } = new List<PartnerReferal>();

        public string SpreadsheetId { get; set; } = string.Empty;

        public string InviteCode{ get; set; } = string.Empty;
        public bool IsActive { get; set; } 
    }
}
