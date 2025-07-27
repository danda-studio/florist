using FloristAI.Application.GoogleDrive;
using FloristAI.Application.GoogleSheets;
using FloristAI.Application.Language;
using FloristAI.Application.Store;
using FloristAI.Application.Users.Models.Request;
using FloristAI.Application.Users.Models.Response;
using FloristAI.Core.Entities.Enums;
using FloristAI.Core.Entities.ReferralsAndPartners;
using FloristAI.Core.Entities.UserInfo;
using FloristAI.Core.Store;
using Google.Apis.Sheets.v4;
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
        private readonly IUserRepository _userRepository;

        private readonly ICacheRepository _cacheRepository;

        private readonly IGoogleSheetsService _googleSheetsService;
        private readonly IGoogleDriveService _googleDriveService;

        /// <summary>
        /// Сервис локализации для получения локализованных строк.
        /// </summary>
        private readonly ILocalizationService _localizationService;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="UserService"/>.
        /// </summary>
        /// <param name="userRepository">Репозиторий пользователей.</param>
        /// <param name="localizationService">Сервис локализации.</param>
        public UserService(IUserRepository userRepository, ILocalizationService localizationService, ICacheRepository cacheRepository, IGoogleSheetsService googleSheetsService, IGoogleDriveService googleDriveService)
        {
            _userRepository = userRepository;
            _localizationService = localizationService;
            _cacheRepository = cacheRepository;
            _googleSheetsService = googleSheetsService;
            _googleDriveService = googleDriveService;
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

            // Получаем текущий прогресс
            var progress = await _cacheRepository.GetProgress(request.ChatId)
                           ?? new PartnerFormProgress { ChatId = request.ChatId };

            // Обновляем только то, что пришло в запросе
            if (request.Step != null)
                progress.Step = request.Step;

            if (!string.IsNullOrWhiteSpace(request.FirstName))
                progress.FirstName = request.FirstName;

            if (!string.IsNullOrWhiteSpace(request.LastName))
                progress.LastName = request.LastName;

            if (!string.IsNullOrWhiteSpace(request.Phone))
                progress.Phone = request.Phone;

            // сохраняем обновлённый прогресс
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

            var isPartner = await _userRepository.IsPartner(chatId);

            return new GetUserResponse
            {
                UserId = user.Id,
                LanguageCode = user.LanguageCode,
                IsPartner = isPartner
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
            var isPartner = await _userRepository.IsPartner(chatId);
            await _userRepository.EditLanguageCode(user.UserId, languageCode);

            return new EditLanguageInterfaceUserResponse
            {
                UserId = user.UserId,
                LanguageCode = languageCode,
                IsPartner = isPartner
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


        public async Task RegisterPartner(long chatId)
        {
            var stepData = await GetStep(chatId) ?? throw new InvalidOperationException($"Step data not found for chatId {chatId}");
            var request = new AddPartnerRequest
            {
                ChatId = stepData.ChatId,
                FirstName = stepData.FirstName ?? string.Empty,
                LastName = stepData.LastName ?? string.Empty,
                PhoneNumber = stepData.Phone ?? string.Empty
            };

            await AddPartner(request);
            await ClearStep(chatId);
        }

        public async Task CreateStructureFolderAndSheetPartner(CreateStructureFolderAndSheetPartnerRequest request)
        {
            //var privateFolderId = await _googleDriveService.GetPrivateFolderId();
            //var partnerFolderId = await _googleDriveService.CreateFolder(request.Name, privateFolderId); 
            //var spreadsheetUrl = await _googleSheetsService.CreateSpreadsheet();

            //var spreadsheetId = ExtractSpreadsheetId(spreadsheetUrl);
            //foreach (var month in GetMonths())
            //{
            //    await _googleSheetsService.AddSheet(spreadsheetId, month);
            //}

            //await _userRepository.EditPartnerInfo();
        }

        private string ExtractSpreadsheetId(string url)
        {
            var parts = url.Split('/');
            var index = Array.IndexOf(parts, "d");
            return index >= 0 && parts.Length > index + 1 ? parts[index + 1] : throw new Exception("Invalid URL");
        }

        private async Task<Partner> AddPartner(AddPartnerRequest request)
        {

            var user = await _userRepository.GetUserByChatId(request.ChatId) ?? throw new Exception("User not found");
            var partner = new Partner
            {
                UserId = user.Id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber
            };

            var spreadsheetName = $"{partner.Id}/{partner.FirstName}/{partner.LastName}/{DateTime.UtcNow:yyyy}";

            return await _userRepository.AddPartner(partner);
        }
    }
}
