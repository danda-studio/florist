using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepFlowBuilder;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
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

        public async Task<List<MessageResult>> BuildMenu(long chatId)
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

            await _userService.RegisterPartner(chatId);

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
