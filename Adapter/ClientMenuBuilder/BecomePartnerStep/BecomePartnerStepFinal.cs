using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepFlowBuilder;
using FloristAI.Application.GoogleSheets.Models.Request;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using FloristAI.Application.Users.Models.Request;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.ClientMenuBuilder.BecomePartnerStep
{
    public class BecomePartnerStepFinal : IStepFlowBuilder
    {
        private readonly IUserService _userService;
        private readonly ILocalizationService _localizationService;

        public BecomePartnerStepFinal(IUserService userService, ILocalizationService localizationService)
        {
            _userService = userService;
            _localizationService = localizationService;
        }

        public string Step => "become_partner_step_final";

        public async Task<List<MessageResult>> BuildMenu(long chatId, string? username = null)
        {
            var user = await _userService.GetUser(chatId);
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

            byte[] qrBytes = _userService.GetReferralQrCode(user.UserId);

            var referralText = $"""
            {_localizationService.GetString("Become_Form_Success", user.LanguageCode)}

            {_localizationService.GetString("Referral_Link_Label", user.LanguageCode)} {_userService.GetReferralLink(user.UserId)}

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

            var userInfo = await _userService.GetStep(chatId); 

            var request = new CreateStructureFolderAndSheetRequest
            {
                PartnerId = user.UserId, 
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
            };

            var sheet = await _userService.CreateStructureFolderAndSheet(request);

            var SheetId = sheet.FirstOrDefault(s => s.FileName == _localizationService.GetString("Total_Info", "sheetName") || s.SheetName == _localizationService.GetString("Total_Info", "sheetName")) ?? throw new Exception("Не удалось найти таблицу");
            var publicSheet = sheet.FirstOrDefault(s => s.IsPublic == true) ?? throw new Exception ("Не удалось найти публичную таблицу");

            await _userService.RegisterPartner(chatId, publicSheet.SpreadsheetId);

            await _userService.AddDataInRow(
                new AddDataRequest
                {
                    UserId = user.UserId,
                    SpreadsheetId = SheetId.SpreadsheetId,
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
