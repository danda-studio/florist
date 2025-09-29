using FloristAI.Core.Entities.Enums;
using FloristAI.Core.Entities.ReferralsAndPartners;
using FloristAI.Core.Entities.UserInfo;
using FloristAI.Core.Store;
using Microsoft.EntityFrameworkCore;

namespace FloristAI.Infrastructure.Persistence
{
    /// <summary>
    /// Репозиторий для работы с пользователями в базе данных PostgreSQL.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly PostgresDbContext _dbContext;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UserRepository"/>.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        public UserRepository(PostgresDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Получает список ролей пользователя по его идентификатору.
        /// </summary>
        /// <param name="Id">Идентификатор пользователя.</param>
        /// <returns>Список ролей пользователя.</returns>
        public async Task<List<UserRole>> GetRoles(int Id)
        {
            return await _dbContext.UserRoles
                .Where(ur => ur.UserId == Id)
                .ToListAsync();
        }

        /// <summary>
        /// Получает пользователя по Telegram chatId.
        /// </summary>
        /// <param name="chatId">Идентификатор чата в Telegram.</param>
        /// <returns>Пользователь или null, если не найден.</returns>
        public async Task<User?> GetUserByChatId(long chatId)
        {
            return await _dbContext.Users
                .Include(u => u.TgData)
                .FirstOrDefaultAsync(u => u.TgData != null && u.TgData.TelegramId == chatId);
        }

        public async Task<Partner?> GetPartnerByInviteCode(string inviteCode)
        {
            return await _dbContext.Partners
                .FirstOrDefaultAsync(p => p.InviteCode == inviteCode);
        }

        /// <summary>
        /// Создаёт пользователя с данными Telegram и языком интерфейса.
        /// </summary>
        /// <param name="chatId">Идентификатор чата Telegram.</param>
        /// <param name="languageCode">Код языка интерфейса.</param>
        /// <returns>Созданный пользователь.</returns>
        public async Task<User> CreateUserWithChatData(long chatId, string languageCode)
        {
            var user = new User
            {
                LanguageCode = languageCode,
                TgData = new UserTgData
                {
                    TelegramId = chatId
                },
                Roles = new List<UserRole>
                {
                    new UserRole
                    {
                        Role = RoleType.Client
                    }
                }
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Изменяет язык интерфейса пользователя.
        /// </summary>
        /// <param name="Id">Идентификатор пользователя.</param>
        /// <param name="languageCode">Новый код языка интерфейса.</param>
        /// <returns>True, если обновление успешно, иначе false.</returns>
        public async Task<bool> EditLanguageCode(int Id, string languageCode)
        {
            var user = await _dbContext.Users.FindAsync(Id);
            if (user == null) return false;

            user.LanguageCode = languageCode;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Partner> AddPartner(Partner partner)
        {
            await _dbContext.Partners.AddAsync(partner);

            if(partner.UserId.HasValue)
            {
                bool hasPartnerRole = await _dbContext.UserRoles
                    .AnyAsync(r => r.UserId == partner.UserId && r.Role == RoleType.Partner);

                if (!hasPartnerRole)
                {
                    _dbContext.UserRoles.Add(new UserRole
                    {
                        UserId = (int)partner.UserId,
                        Role = RoleType.Partner
                    });
                }
            }

            await _dbContext.SaveChangesAsync();

            return partner;
        }
        public async Task UpdatePartner(Partner partner)
        {
            var partnerInfo  = await ResolvePartnerInvite(partner.InviteCode) 
                ?? throw new InvalidOperationException($"Партнёр с InviteCode {partner.InviteCode} не найден");

            partnerInfo.UserId = partner.UserId;
            partnerInfo.SpreadsheetId = partner.SpreadsheetId ?? partnerInfo.SpreadsheetId;
            partnerInfo.IsActive = partner.IsActive;
            partnerInfo.PrivateSpreadsheetId = partner.PrivateSpreadsheetId;


            if (partner.UserId.HasValue)
            {
                bool hasPartnerRole = await _dbContext.UserRoles
                    .AnyAsync(r => r.UserId == partner.UserId && r.Role == RoleType.Partner);

                if (!hasPartnerRole)
                {
                    _dbContext.UserRoles.Add(new UserRole
                    {
                        UserId = (int)partner.UserId,
                        Role = RoleType.Partner
                    });
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsPartner(long chatId)
        {
            var userId = await _dbContext.UserTgDatas
                .Where(t => t.TelegramId == chatId)
                .Select(t => t.UserId)
                .FirstOrDefaultAsync();

            if (userId == 0)
                return false;

            return await _dbContext.Partners
                .AnyAsync(p => p.UserId == userId);
        }


        public async Task<string?> GetSpreadsheetId(int userId)
        {
            return await _dbContext.Partners
                 .Where(p => p.UserId == userId)
                 .Select(p => p.SpreadsheetId)
                 .FirstOrDefaultAsync();
        }

        public async Task<Referal> AddReferal(Referal referal, int partnerId)
        {
            // Проверка существующего реферала
            var existing = await _dbContext.Referals
                .Include(r => r.PartnerReferal)
                .FirstOrDefaultAsync(r => r.ReferalId == referal.ReferalId);

            if (existing != null)
                return existing;

            var partner = await _dbContext.Partners.FirstOrDefaultAsync(p => p.UserId == partnerId) ?? throw new InvalidOperationException($"Партнёр с UserId {partnerId} не найден");
            referal.PartnerReferal = new PartnerReferal
            {
                PartnerId = partner.Id,  
                ReferalId = referal.ReferalId
            };

            await _dbContext.Referals.AddAsync(referal);
            await _dbContext.SaveChangesAsync();

            return referal;
        }

        public async Task<Partner?> ResolvePartnerInvite(string code)
        {
            return await _dbContext.Partners
                .FirstOrDefaultAsync(x => x.InviteCode == code);
        }


    }
}
