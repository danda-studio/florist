using FloristAI.Application.GoogleSheets.Models.Request;
using FloristAI.Application.GoogleSheets.Models.Response;
using FloristAI.Application.Store.Models.Response;
using FloristAI.Application.Users.Models.Request;
using FloristAI.Application.Users.Models.Response;
using FloristAI.Core.Entities.ReferralsAndPartners;

namespace FloristAI.Application.Users
{
    /// <summary>
    /// Интерфейс сервиса для работы с пользователями.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Получает информацию о пользователе по chatId Telegram.
        /// </summary>
        /// <param name="chatId">Идентификатор чата Telegram.</param>
        /// <returns>Модель с данными пользователя.</returns>
        Task<GetUserResponse> GetUser(long chatId);
        Task<GetUserResponse> GetOrCreateUser(long chatId, string languageCode);
        Task<GetStepResponse> GetStep(long chatId);
        string GetReferralLink(int Id);
        Task<string> GetPartnerLink(GetPartnerLinkRequest request);
        byte[] GetReferralQrCode(int Id);
        byte[] GetPartnerLinkQrCode(string link);


        /// <summary>
        /// Получает список ролей пользователя по его Telegram chatId.
        /// </summary>
        /// <param name="chatId">Идентификатор чата пользователя в Telegram.</param>
        /// <param name="languageCode">Код языка интерфейса для локализации ролей.</param>
        /// <returns>Модель с ролями пользователя.</returns>
        Task<GetRolesResponse> GetRolesByTelegramId(long chatId, string languageCode);

        /// <summary>
        /// Создаёт пользователя с указанным chatId и языком интерфейса.
        /// </summary>
        /// <param name="chatId">Идентификатор чата Telegram.</param>
        /// <param name="languageCode">Код языка интерфейса.</param>
        /// <returns>Ответ с информацией о созданном пользователе.</returns>
        Task<AddUserResponse> AddUser(long chatId, string languageCode);
        Task<Partner> AddPartner(AddPartnerRequest request);
        Task UpdatePartnerOnActivation(long chatId, string spreadSheetId, string inviteCode);
        Task<AddDataInRowResponse> AddDataInRow(AddDataRequest request);

        /// <summary>
        /// Изменяет язык интерфейса пользователя.
        /// </summary>
        /// <param name="chatId">Идентификатор чата Telegram пользователя.</param>
        /// <param name="languageCode">Новый код языка интерфейса.</param>
        /// <returns>Ответ с подтверждением изменения языка.</returns>
        Task<EditLanguageInterfaceUserResponse> EditLanguageInterfaceUser(long chatId, string languageCode);
        Task<List<CreateStructureSheetResponse>> CreateStructureFolderAndSheet(CreateStructureFolderAndSheetRequest request);
        Task<bool> SaveStep(SaveStepRequest request);
        Task<bool> ClearStep(long chatId);
        Task RegisterPartner(long chatId, string spreadSheetId);
        Task ProcessReferral(ProcessReferralRequest request);
        Task<ResolvePartnerInviteResponse> ResolvePartnerInvite(string code);

    }
}
