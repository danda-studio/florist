using FloristAI.Application.Language;
using FloristAI.Application.User.Models;
using FloristAI.Application.User.Models.Response;
using FloristAI.Core.Store;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FloristAI.Application.User
{
    /// <summary>
    /// Сервис для работы с пользователями
    /// </summary>
    public class UserService : IUserService
    {
        /// <summary>
        /// Репозиторий для работы с данными пользователей.
        /// </summary>
        public readonly IUserRepository _userRepository;

        /// <summary>
        /// Сервис локализации для получения локализованных строк.
        /// </summary>
        public readonly ILocalizationService _localizationService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UserService"/>.
        /// </summary>
        /// <param name="userRepository">Репозиторий пользователей.</param>
        /// <param name="localizationService">Сервис локализации.</param>
        public UserService(IUserRepository userRepository, ILocalizationService localizationService)
        {
            _userRepository = userRepository;
            _localizationService = localizationService;
        }

        /// <summary>
        /// Добавляет пользователя с указанным chatId и языком интерфейса.
        /// </summary>
        /// <param name="chatId">Идентификатор Telegram-чата пользователя.</param>
        /// <param name="languageCode">Код языка интерфейса пользователя.</param>
        /// <returns>Информация о созданном или существующем пользователе.</returns>
        public async Task<AddUserResponse> AddUser(long chatId, string languageCode)
        {
            var user = await _userRepository.CreateUserWithChatData(chatId, languageCode);

            return new AddUserResponse
            {
                Id = user.Id,
                LanguageCode = user.LanguageCode
            };
        }


        private async Task<bool> CheckUserInSystem(long chatId)
        {
            var user = await _userRepository.GetUserByChatId(chatId);
            if (user == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Получает информацию о пользователе по chatId.
        /// </summary>
        /// <param name="chatId">Идентификатор Telegram-чата пользователя.</param>
        /// <returns>Информация о пользователе.</returns>
        /// <exception cref="InvalidOperationException">Если пользователь не найден.</exception>
        public async Task<GetUserResponse> GetUser(long chatId)
        {
            var user = await _userRepository.GetUserByChatId(chatId)
                ?? throw new InvalidOperationException($"Пользователь с chatId {chatId} не найден");

            return new GetUserResponse
            {
                UserId = user.Id,
                LanguageCode = user.LanguageCode
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

            GetUserResponse user;

            if (!await CheckUserInSystem(chatId))
            {
                var createdUser = await AddUser(chatId, languageCode);
                user = new GetUserResponse
                {
                    UserId = createdUser.Id,
                    LanguageCode = createdUser.LanguageCode
                };
            }
            else
            {
                user = await GetUser(chatId);
                if (user == null)
                {
                    throw new InvalidOperationException($"Пользователь с chatId {chatId} не найден");
                }
            }
            var roles = await _userRepository.GetRoles(user.UserId);
            var response = roles
            .Select(r => new UserRole
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
        /// Изменяет язык интерфейса пользователя.
        /// </summary>
        /// <param name="chatId">Идентификатор Telegram-чата пользователя.</param>
        /// <param name="languageCode">Новый код языка интерфейса.</param>
        /// <returns>Результат изменения с информацией о пользователе и новом языке.</returns>
        public async Task<EditLanguageInterfaceUserResponse> EditLanguageInterfaceUser(long chatId, string languageCode)
        {
            var user = await GetUser(chatId);
            var success = await _userRepository.EditLanguageCode(user.UserId, languageCode);

            return new EditLanguageInterfaceUserResponse
            {
                UserId = user.UserId,
                LanguageCode = languageCode
            };
        }
    }
}
