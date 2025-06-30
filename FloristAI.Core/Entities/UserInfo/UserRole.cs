using FloristAI.Core.Entities.Enums;
using System;

namespace FloristAI.Core.Entities.UserInfo
{
    /// <summary>
    /// Представляет роль пользователя в системе.
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// Идентификатор пользователя, которому назначена роль.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Тип роли, назначенной пользователю (например, Клиент, Админ и т.д.).
        /// </summary>
        public RoleType Role { get; set; } 

        /// <summary>
        /// Пользователь, связанный с данной ролью.
        /// </summary>
        public User? User { get; set; }
    }
}
