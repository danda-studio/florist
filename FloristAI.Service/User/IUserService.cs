using FloristAI.Application.User.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.User
{
    public interface IUserService
    {
        public Task<GetRolesResponse> GetRoles(int Id);
    }
}
