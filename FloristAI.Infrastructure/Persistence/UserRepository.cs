using FloristAI.Core.Entities.UserInfo;
using FloristAI.Core.Store;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Infrastructure.Persistence
{
    public class UserRepository : IUserRepository
    {
        public readonly PostgresDbContext _dbContext;

        public UserRepository(PostgresDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<UserRole>> GetRoles (int Id)
        {
            return await _dbContext.UserRoles
                           .Where(ur => ur.UserId == Id)
                           .ToListAsync();
        }

        public async Task<User> AddUser ()
        {

        }
    }
}
