using FloristAI.Core.Entities.UserInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Core.Store
{
    public interface IUserRepository
    {
        public Task<List<UserRole>> GetRoles(int Id);
        public Task<User> CreateUserWithChatData(long chatId);
        public Task<User?> GetUserByChatId(long chatId);


    }
}
