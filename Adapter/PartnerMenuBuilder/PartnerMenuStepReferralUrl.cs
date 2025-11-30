using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepMenuBuilder;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.PartnerMenuBuilder
{
    public class PartnerMenuStepReferralUrl : IStepMenuBuilder
    {
        private readonly IUserService _userService;
        private readonly IPartnerService _partnerService;
        private readonly ILocalizationService _localizationService;
        public PartnerMenuStepReferralUrl(IUserService userService, IPartnerService partnerService, ILocalizationService localizationService)
        {
            _userService = userService;
            _partnerService = partnerService;
            _localizationService = localizationService;
        }
        public string Step => "referal_url";

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

            byte[] qrBytes = _partnerService.GetReferralQrCode(user.UserId);
            var referralText = $"""

            {_localizationService.GetString("Referral_Link_Label", user.LanguageCode)} {_partnerService.GetReferralLink(user.UserId)}

            {_localizationService.GetString("Referral_Description", user.LanguageCode)}
            """;

            var keyboard = new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Button_Back", user.LanguageCode), "role_menu:Partner") },
            };

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
                    ReplyMarkup = new InlineKeyboardMarkup(keyboard)
                }
            };
        }
    }
}
