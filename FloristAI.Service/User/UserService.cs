using FloristAI.Application.User.Models;
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


        public async Task<GetRolesResponse> GetRoles(int Id)
        {
            var roles = await _userRepository.GetRoles(Id);

            var response = roles
                .Select(r => new UserRole
                { 
                    RoleType = r.Role
                })
                .ToList();

            return new GetRolesResponse
            {
                UserId = Id,
                Roles = response
            };

        }
    }
}
