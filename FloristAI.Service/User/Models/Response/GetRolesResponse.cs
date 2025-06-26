using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.User.Models.Response
{
    public class GetRolesResponse
    {
        public int UserId { get; set; }
        public List<UserRole> Roles { get; set; }
    }
}
