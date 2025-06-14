using System;

namespace FloristAI.Core.Entities.Referrals
{
    /// <summary>
    /// Модель связи между партнёром и пользователем в рамках реферальной программы.
    /// </summary>
    public class PartnerReferal
    {
        /// <summary>
        /// Идентификатор партнёра, пригласившего пользователя.
        /// </summary>
        public int PartnerId { get; set; }

        /// <summary>
        /// Идентификатор пользователя, приглашённого партнёром.
        /// </summary>
        public int ReferalId { get; set; }

        /// <summary>
        /// Реферальная запись, связанная с данной связью партнёр–пользователь.
        /// </summary>
        public Referal Referal { get; set; }

        /// <summary>
        /// Партнёр, пригласивший пользователя.
        /// </summary>
        public Partner Partner { get; set; }
    }
}
