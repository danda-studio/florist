using FloristAI.Application.Language;
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
        public readonly ILocalizationService _localizationService;
        public UserService(IUserRepository userRepository, ILocalizationService localizationService) 
        {
            _userRepository = userRepository;
            _localizationService = localizationService;
        }


        public async Task<GetOrCreateUserResponse> GetOrCreateUser(long chatId, string languageCode)
        {
            var user = await _userRepository.GetUserByChatId(chatId);
            var entity = user ?? await _userRepository.CreateUserWithChatData(chatId, languageCode);

            return new GetOrCreateUserResponse
            {
                Id = entity.Id,
                LanguageCode = entity.LanguageCode
            };
        }

        public async Task<GetRolesResponse> GetRolesByTelegramId(long chatId, string languageCode)
        {
            var user = await GetOrCreateUser(chatId,languageCode);
            var roles = await _userRepository.GetRoles(user.Id);

            var response = roles
                .Select(r => new UserRole
                { 
                    RoleType = r.Role,
                    RoleName = _localizationService.GetString($"Role_{r.Role}", languageCode)
                })
                .ToList();

            return new GetRolesResponse
            {
                UserId = user.Id,
                Roles = response
            };

        }

        public async Task<bool> EditLanguageInterfaceUser (long chatId, string languageCode)
        {
            var user = await GetOrCreateUser(chatId, languageCode);
            return await _userRepository.EditLanguageCode(user.Id, languageCode);

        }
    }
}
