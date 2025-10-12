using AutoMapper;
using FloristAI.Application.Language;
using FloristAI.Application.Store;
using FloristAI.Application.Users.Models.Response;


namespace FloristAI.Application.Users
{
    /// <summary>
    /// Сервис для работы с пользователями
    /// </summary>
    public class UserService : IUserService
    {
        /// <summary>
        /// Репозитории для работы с данными пользователей.
        /// </summary>
        private readonly IUserRepository _userRepository;
        private readonly IPartnerRepository _partnerRepository;

        /// <summary>
        /// Сервис локализации для получения локализованных строк.
        /// </summary>
        private readonly ILocalizationService _localizationService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UserService"/>.
        /// </summary>
        /// <param name="userRepository">Репозиторий пользователей.</param>
        /// <param name="localizationService">Сервис локализации.</param>
        public UserService(IUserRepository userRepository, ILocalizationService localizationService, IPartnerRepository partnerRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _localizationService = localizationService;
            _partnerRepository = partnerRepository;
            _mapper = mapper;
        }

        public async Task<GetUserResponse> GetOrCreateUser(long chatId, string languageCode, bool IsModerator)
        {
            if (!await CheckUserInSystem(chatId))
            {
                var createdUser = await AddUser(chatId, languageCode, IsModerator);
                return new GetUserResponse
                {
                    UserId = createdUser.Id,
                    LanguageCode = createdUser.LanguageCode
                };
            }

            var user = await GetUser(chatId);
            return user ?? throw new InvalidOperationException($"Пользователь с chatId {chatId} не найден");
        }

        /// <summary>
        /// Получает информацию о пользователе по chatId.
        /// </summary>
        /// <param name="chatId">Идентификатор Telegram-чата пользователя.</param>
        /// <returns>Информация о пользователе.</returns>
        /// <exception cref="InvalidOperationException">Если пользователь не найден.</exception>
        public async Task<GetUserResponse> GetUser(long chatId)
        {
            var user = await _userRepository.GetUserByChatId(chatId);
            if(user == null)
            {
                return new GetUserResponse();
            }

            var isPartner = await _partnerRepository.IsPartner(chatId);

            return new GetUserResponse
            {
                UserId = user.Id,
                LanguageCode = user.LanguageCode,
                IsPartner = isPartner,
                Roles = user.Roles.Select(r => r.Role.ToString()).ToList()
            };
        }

        /// <summary>
        /// Получает роли пользователя по chatId и языковому коду, создаёт пользователя при необходимости.
        /// </summary>
        /// <param name="chatId">Идентификатор Telegram-чата пользователя.</param>
        /// <param name="languageCode">Код языка интерфейса.</param>
        /// <returns>Роли пользователя с локализованными названиями.</returns>
        public async Task<GetRolesResponse> GetRolesByTelegramId(long chatId, string languageCode)
        {
            var user = await GetOrCreateUser(chatId, languageCode, false);

            var roles = await _userRepository.GetRoles(user.UserId);
            var response = roles
                .Select(r => new Models.Response.UserRole
                {
                    RoleType = r.Role,
                    RoleName = _localizationService.GetString($"Role_{r.Role}", languageCode)
                })
                .ToList();

            return new GetRolesResponse
            {
                UserId = user.UserId,
                Roles = response
            };
        }

        /// <summary>
        /// Добавляет пользователя с указанным chatId и языком интерфейса.
        /// </summary>
        /// <param name="chatId">Идентификатор Telegram-чата пользователя.</param>
        /// <param name="languageCode">Код языка интерфейса пользователя.</param>
        /// <returns>Информация о созданном или существующем пользователе.</returns>
        public async Task<AddUserResponse> AddUser(long chatId, string languageCode, bool IsModerator)
        {
            var user = await _userRepository.CreateUserWithChatData(chatId, languageCode, IsModerator);

            return new AddUserResponse
            {
                Id = user.Id,
                LanguageCode = user.LanguageCode
            };
        }

        /// <summary>
        /// Изменяет язык интерфейса пользователя.
        /// </summary>
        /// <param name="chatId">Идентификатор Telegram-чата пользователя.</param>
        /// <param name="languageCode">Новый код языка интерфейса.</param>
        /// <returns>Результат изменения с информацией о пользователе и новом языке.</returns>
        public async Task<EditLanguageInterfaceUserResponse> EditLanguageInterfaceUser(long chatId, string languageCode)
        {
            var user = await GetUser(chatId);
            var isPartner = await _partnerRepository.IsPartner(chatId);
            await _userRepository.EditLanguageCode(user.UserId, languageCode);

            return new EditLanguageInterfaceUserResponse
            {
                UserId = user.UserId,
                LanguageCode = languageCode,
                IsPartner = isPartner
            };
        }

        /// <summary>
        /// Проверка, есть ли пользователь в системе по chatId.
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        private async Task<bool> CheckUserInSystem(long chatId)
        {
            var user = await _userRepository.GetUserByChatId(chatId);
            if (user == null)
            {
                return false;
            }

            return true;
        }
    }
}
