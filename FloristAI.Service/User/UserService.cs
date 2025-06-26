using FloristAI.Application.User.Models;
using FloristAI.Application.User.Models.Response;
using FloristAI.Core.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.User
{
    public class UserService : IUserService
    {
        public readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository) 
        {
            _userRepository = userRepository;
        }


        public async Task<GetOrCreateUserResponse> GetOrCreateUser(long chatId)
        {
            var user = await _userRepository.GetUserByChatId(chatId);
            var entity = user ?? await _userRepository.CreateUserWithChatData(chatId);

            return new GetOrCreateUserResponse
            {
                Id = entity.Id,
                LanguageCode = entity.LanguageCode
            };
        }

        public async Task<GetRolesResponse> GetRolesByTelegramId(long chatId)
        {
            // 1. Получаем пользователя за один запрос
            var user = await _userRepository.GetUserWithRolesByChatId(chatId)
                       ?? await _userRepository.CreateUserWithChatData(chatId);

            // 2. Преобразуем роли
            var roles = user.Roles.Select(r => new UserRole
            {
                RoleType = r.RoleType
            }).ToList();

            return new GetRolesResponse
            {
                UserId = user.Id,
                Roles = roles
            };
        }
    }
}
