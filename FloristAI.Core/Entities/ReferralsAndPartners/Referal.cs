using FloristAI.Core.Entities.UserInfo;
using System;

namespace FloristAI.Core.Entities.ReferralsAndPartners
{
    /// <summary>
    /// Модель реферала (пользователь)
    /// </summary>
    public class Referal
    {
        /// <summary>
        /// Уникальный идентификатор.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя, связанного с рефералом.
        /// </summary>
        public int ReferalId { get; set; }

        /// <summary>
        /// Cвязь между партнёром и пользователем в рамках реферальной программы
        /// </summary>
        public PartnerReferal PartnerReferal { get; set; } = new PartnerReferal();

        /// <summary>
        /// Пользователь, которому принадлежит статус реферала.
        /// </summary>
        public User User { get; set; }
    }
}