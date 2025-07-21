using FloristAI.Application.Language;
using FloristAI.Application.Users.Models.Request;
using FloristAI.Application.Users.Models.Response;
using FloristAI.Core.Entities.Enums;
using FloristAI.Core.Entities.ReferralsAndPartners;
using FloristAI.Core.Entities.UserInfo;
using FloristAI.Core.Store;
using QRCoder;

namespace FloristAI.Application.Users
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

        public readonly ICacheRepository _cacheRepository;

        /// <summary>
        /// Сервис локализации для получения локализованных строк.
        /// </summary>
        public readonly ILocalizationService _localizationService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UserService"/>.
        /// </summary>
        /// <param name="userRepository">Репозиторий пользователей.</param>
        /// <param name="localizationService">Сервис локализации.</param>
        public UserService(IUserRepository userRepository, ILocalizationService localizationService, ICacheRepository cacheRepository)
        {
            _userRepository = userRepository;
            _localizationService = localizationService;
            _cacheRepository = cacheRepository;
        }


        private async Task<GetUserResponse> GetOrCreateUser(long chatId, string languageCode)
        {
            if (!await CheckUserInSystem(chatId))
            {
                var createdUser = await AddUser(chatId, languageCode);
                return new GetUserResponse
                {
                    UserId = createdUser.Id,
                    LanguageCode = createdUser.LanguageCode
                };
            }

            var user = await GetUser(chatId);
            return user ?? throw new InvalidOperationException($"Пользователь с chatId {chatId} не найден");
        }

        public async Task<GetStepResponse> GetStep(long chatId)
        {
           var step = await _cacheRepository.GetProgress(chatId);

            return new GetStepResponse
            {
                ChatId = chatId,
                Step = step?.Step, 
                FirstName = step?.FirstName,
                LastName = step?.LastName,
                Phone = step?.Phone
            };

        }

        public async Task<bool> SaveStep(SaveStepRequest request)
        {
            if (request == null || request.ChatId <= 0)
            {
                throw new ArgumentException("Неверный запрос для сохранения шага");
            }

            // Получаем текущий прогресс, если есть, иначе создаём новый
            var progress = await _cacheRepository.GetProgress(request.ChatId);
            if (progress == null)
            {
                progress = new PartnerFormProgress { ChatId = request.ChatId };
            }

            // Обновляем только переданные поля
            if (!string.IsNullOrEmpty(request.FirstName))
                progress.FirstName = request.FirstName;

            if (!string.IsNullOrEmpty(request.LastName))
                progress.LastName = request.LastName;

            if (!string.IsNullOrEmpty(request.Phone))
                progress.Phone = request.Phone;

            if (!string.IsNullOrEmpty(request.Step))
                progress.Step = request.Step;

            return await _cacheRepository.SaveProgress(progress);
        }



        public async Task<bool> ClearStep(long chatId)
        {
            if (chatId <= 0)
            {
                throw new ArgumentException("Неверный идентификатор чата");
            }
            return await _cacheRepository.ClearProgress(chatId);
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
            var user = await GetOrCreateUser(chatId, languageCode);

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

        public string GetReferralLink(int Id)
        {
            string botName = "FLowerKisaBot";
            return $"https://t.me/{botName}?start={Id}";
        }

        public byte[] GetReferralQrCode(int id)
        {
            string link = GetReferralLink(id);

            var generator = new QRCodeGenerator();
            var data = generator.CreateQrCode(link, QRCodeGenerator.ECCLevel.Q);

            var renderer = new PngByteQRCode(data);
            return renderer.GetGraphic(20);
        }


        public async Task<Partner> AddPartner(AddPartnerRequest request)
        {
            var user = await _userRepository.GetUserByChatId(request.ChatId);
            if (user == null)
                throw new Exception("User not found");

            var partner = new Partner
            {
                UserId = user.Id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber
            };

            return await _userRepository.AddPartner(partner);
        }

    }
}
