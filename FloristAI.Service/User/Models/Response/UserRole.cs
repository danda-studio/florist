using FloristAI.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.User.Models.Response
{
    public class UserRole
    {
        public RoleType RoleType { get; set; } = RoleType.Client;
        public string RoleName { get; set; } = "";
    }
}
