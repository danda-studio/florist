using FloristAI.Application.User.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.User
{
    public interface IUserService
    {
        public Task<GetRolesResponse> GetRolesByTelegramId(long chatId);

        public Task<GetOrCreateUserResponse> GetOrCreateUser(long chatId);
    }
}
