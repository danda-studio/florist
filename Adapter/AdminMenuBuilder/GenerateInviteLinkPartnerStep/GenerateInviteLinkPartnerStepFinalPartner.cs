using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepFlowBuilder;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using FloristAI.Application.Users.Models.Request;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.AdminMenuBuilder.GenerateInviteLinkPartnerStep
{
    public class GenerateInviteLinkPartnerStepFinalPartner : IStepFlowBuilder
    {
        private readonly IUserService _userService;
        private readonly IPartnerService _partnerService;

        private readonly IStepFlowService _stepFlowService;
        private readonly ILocalizationService _localizationService;

        public GenerateInviteLinkPartnerStepFinalPartner(IUserService userService, IPartnerService partnerService, IStepFlowService stepFlowService, ILocalizationService localizationService)
        {
            _userService = userService;
            _partnerService = partnerService;
            _stepFlowService = stepFlowService;
            _localizationService = localizationService;
        }

        public string Step => "generate_partnerLink_final";

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

            var keyboard = new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        _localizationService.GetString("Button_Menu", user.LanguageCode),
                        "role_menu:Admin"
                    )
                }
            };

            var userInfo = await _stepFlowService.GetStep(chatId);

            var mappingDataForLink = new GetPartnerLinkRequest
            {
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                Phone = userInfo.Phone,
            };

            var partnerLink = await _partnerService.GetPartnerLink(mappingDataForLink);
            byte[] qrBytes = _partnerService.GetPartnerLinkQrCode(partnerLink);

            var referralText = $"""
            {_localizationService.GetString("Generate_PartnerLink_Success", user.LanguageCode)} {partnerLink}
            """;


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
