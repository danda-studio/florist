using FloristAI.Core.Entities.UserInfo;
using System.Collections.Generic;

namespace FloristAI.Core.Entities.Referrals
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
        public int UserId { get; set; }

        /// <summary>
        /// Пользователь, которому принадлежит статус партнёра.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Список рефералов, приглашённых данным партнёром.
        /// </summary>
        public List<PartnerReferal> Partners { get; set; } = new List<PartnerReferal>();
    }
}
