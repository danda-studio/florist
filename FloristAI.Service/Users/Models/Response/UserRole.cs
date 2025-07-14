using FloristAI.Core.Entities.Enums;
using System;

namespace FloristAI.Application.Users.Models.Response
{
    /// <summary>
    /// Модель, представляющая роль пользователя.
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// Тип роли пользователя.
        /// </summary>
        public RoleType RoleType { get; set; } = RoleType.Client;

        /// <summary>
        /// Локализованное название роли пользователя.
        /// </summary>
        public string RoleName { get; set; } = string.Empty;
    }
}
