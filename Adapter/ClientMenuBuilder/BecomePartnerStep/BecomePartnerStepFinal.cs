using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepFlowBuilder;
using FloristAI.Adapter.StepMenuBuilder;
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

        public async Task<MessageResult> BuildMenu(long chatId)
        {
            var user = await _userService.GetUser(chatId);
            if (user == null)
            {
                return new MessageResult
                {
                    Text = _localizationService.GetString("UserNotFound", "ru"),
                    ReplyMarkup = null
                };
            }

           /* var qrImageUrl = GenerateQrImageUrl(user.ReferralLink);*/ // QR-картинка от ссылки
            var referralText = $"""
                ✅ {_localizationService.GetString("Become_Form_Success", user.LanguageCode)}

                🔗 {_localizationService.GetString("Referral_Link_Label", user.LanguageCode)}: "какой-то урл"

                ℹ️ {_localizationService.GetString("Referral_Description", user.LanguageCode)}
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
                    _localizationService.GetString("Button_MainMenu", user.LanguageCode),
                    "menu"
                )
            }
        };

            return new MessageResult
            {
                Text = referralText,
                //Photo = qrImageUrl,
                ReplyMarkup = keyboard
            };
        }

        public async Task<MessageResult> HandleInput(string input, long chatId)
        {
            // Никакой обработки ввода — просто повторяем финальное сообщение
            return await BuildMenu(chatId);
        }
    }

}
