using FloristAI.Core.Entities.Referrals;
using System.Collections.Generic;

namespace FloristAI.Core.Entities.UserInfo
{
    /// <summary>
    /// Модель пользователя
    /// </summary>
    public class User
    {
        /// <summary>
        /// Уникальный идентификатор пользователя.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Список ролей, назначенных пользователю.
        /// </summary>
        public List<UserRole> Role { get; set; } = new List<UserRole>();

        /// <summary>
        /// Информация о пользователе, связанная с Telegram.
        /// </summary>
        public UserTgData TgInfo { get; set; }

        /// <summary>
        /// Информация о партнёре, если пользователь является партнёром.
        /// </summary>
        public Partner Partner { get; set; }

        /// <summary>
        /// Информация о реферале, если пользователь был приглашён.
        /// </summary>
        public Referal Referal { get; set; }

        /// <summary>
        /// Список заказов, оформленных пользователем.
        /// </summary>
        public List<Order> Order { get; set; } = new List<Order>();

        /// <summary>
        /// Список транзакций, связанных с пользователем.
        /// </summary>
        public List<Transaction> Transaction { get; set; } = new List<Transaction>();
    }
}
