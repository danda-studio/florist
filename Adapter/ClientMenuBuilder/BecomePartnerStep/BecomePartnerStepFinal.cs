using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepFlowBuilder;
using FloristAI.Application.GoogleDrive;
using FloristAI.Application.GoogleSheets;
using FloristAI.Application.GoogleSheets.Models.Request;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using FloristAI.Application.Users.Models.Request;
using FloristAI.Core.Entities.ReferralsAndPartners;
using Google.Apis.Sheets.v4.Data;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.ClientMenuBuilder.BecomePartnerStep
{
    public class BecomePartnerStepFinal : IStepFlowBuilder
    {
        private readonly IUserService _userService;
        private readonly IPartnerService _partnerService;

        private readonly IStepFlowService _stepFlowService;
        private readonly ILocalizationService _localizationService;

        private readonly IGoogleSheetsService _googleSheetsService;
        private readonly IGoogleDriveService _googleDriveService;

        public BecomePartnerStepFinal(
            IUserService userService,
            IPartnerService partnerService,
            ILocalizationService localizationService, 
            IStepFlowService stepFlowService, 
            IGoogleSheetsService googleSheetsService, 
            IGoogleDriveService googleDriveService)
        {
            _userService = userService;
            _partnerService = partnerService;
            _localizationService = localizationService;
            _stepFlowService = stepFlowService;
            _googleSheetsService = googleSheetsService;
            _googleDriveService = googleDriveService;
        }

        public string Step => "become_partner_step_final";

        public async Task<List<MessageResult>> BuildMenu(long chatId, string? username = null)
        {
            var user = await _userService.GetOrCreateUser(chatId, "ru", false);
            if (user == null)
            {
                return new List<MessageResult>
                {
                    new MessageResult
                    {
                        Text = _localizationService.GetString("UserNotFound", "ru"),
                        ReplyMarkup = null
                    }
                };
            }

            byte[] qrBytes = _partnerService.GetReferralQrCode(user.UserId);

            var referralText = $"""
            {_localizationService.GetString("Become_Form_Success", user.LanguageCode)}

            {_localizationService.GetString("Referral_Link_Label", user.LanguageCode)} {_partnerService.GetReferralLink(user.UserId)}

            {_localizationService.GetString("Referral_Description", user.LanguageCode)}
            """;

            var keyboard = new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        _localizationService.GetString("GoToPartner_Menu", user.LanguageCode),
                        "role_menu:Partner"
                    )
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        _localizationService.GetString("Button_Menu", user.LanguageCode),
                        "role_menu:Client"
                    )
                }
            };

            var userInfo = await _stepFlowService.GetStep(chatId);

            var folder = await _googleDriveService.CreateStructureFolder();
            var request = new SheetsCreationParams(
                PartnerId: user.UserId,
                FirstName: userInfo.FirstName,
                LastName: userInfo.LastName,
                PublicFolderId: folder.PublicFolderId,
                PrivateFolderId: folder.PrivateFolderId);

            var sheet = await _googleSheetsService.CreateStructureSheet(request);

            var SheetId = sheet.FirstOrDefault(s => s.FileName == _localizationService.GetSheetName("General_Info") || s.SheetName == _localizationService.GetSheetName("General_Info")) ?? throw new Exception("Не удалось найти таблицу");
            var privateSheet = sheet.FirstOrDefault(s => s.FileName != _localizationService.GetSheetName("General_Info") && s.IsPublic == false) ?? throw new Exception("Не удалось найти приватную таблицу");
            var publicSheet = sheet.FirstOrDefault(s => s.IsPublic == true) ?? throw new Exception("Не удалось найти публичную таблицу");

            await _partnerService.RegisterPartner(chatId, publicSheet.SpreadsheetId, privateSheet.SpreadsheetId);

            await _googleSheetsService.AddDataInRowPartnerTable(
                new AddDataRequest
                {
                    UserId = user.UserId,
                    SpreadsheetId = SheetId.SpreadsheetId,
                    PrivateSpreadsheetId = privateSheet.SpreadsheetId,
                    SheetName = SheetId.SheetName,
                    UserData = new UserData
                    {
                        NameAndSurname = $"{userInfo.FirstName} {userInfo.LastName}",
                        PhoneNumber = userInfo.Phone,
                        TelegramId = chatId,
                        TelegramUsername = userInfo.Username
                    }
                });

            return new List<MessageResult>
            {
                new MessageResult
                {
                    Text = referralText,
                    Photo = new PhotoContent
                    {
                        ImageBytes = qrBytes,
                        Description = referralText
                    },
                    ReplyMarkup = new InlineKeyboardMarkup(keyboard),
                    RemovePinnedMessage = true
                }
            };
        }

        public async Task<List<MessageResult>> HandleInput(string input, long chatId)
        {
            return await BuildMenu(chatId);
        }
    }

}
