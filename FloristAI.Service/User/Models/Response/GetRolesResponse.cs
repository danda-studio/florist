using System;
using System.Collections.Generic;

namespace FloristAI.Application.User.Models.Response
{
    /// <summary>
    /// Ответ сервиса, содержащий список ролей пользователя.
    /// </summary>
    public class GetRolesResponse
    {
        /// <summary>
        /// Идентификатор пользователя, для которого получены роли.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Список ролей пользователя.
        /// </summary>
        public List<UserRole> Roles { get; set; }
    }
}
