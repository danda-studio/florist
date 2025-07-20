using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepFlowBuilder;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using FloristAI.Application.Users.Models.Request;
using FloristAI.Application.Validation;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.ClientMenuBuilder.BecomePartnerStep
{
    public class BecomePartnerStepPhone : IStepFlowBuilder
    {
        private readonly IUserService _userService;

        private readonly ILocalizationService _localizationService;

        private readonly Lazy<IStepFlowProvider> _menuProvider;
        public BecomePartnerStepPhone(IUserService userService, ILocalizationService localizationService, Lazy<IStepFlowProvider> menuProvider)
        {
            _userService = userService;
            _localizationService = localizationService;
            _menuProvider = menuProvider;
        }

        public string Step => "become_partner_step_phone";

        public async Task<MessageResult> BuildMenu(long chatId)
        {
            var user = await _userService.GetUser(chatId);
            if (user == null)
            {
                return new MessageResult
                {
                    Text = _localizationService.GetString("UserNotFound", user.LanguageCode),
                    ReplyMarkup = null
                };
            }
            var keyboard = new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Button_Back", user.LanguageCode), "step_message:become_partner_step_lastName") },
            };
            return new MessageResult
            {
                Text = _localizationService.GetString("Become_Input_Phone", user.LanguageCode),
                ReplyMarkup = keyboard
            };
        }

        public async Task<MessageResult> HandleInput(string input, long chatId)
        {
            var user = await _userService.GetUser(chatId);
            if (user == null)
            {
                return new MessageResult
                {
                    Text = _localizationService.GetString("UserNotFound", user.LanguageCode),
                    ReplyMarkup = null
                };
            }

            if (!Validator.IsValidPhone(input))
            {
                return new MessageResult
                {
                    Text = _localizationService.GetString("Phone_Validation_Error", user.LanguageCode),
                    ReplyMarkup = null
                };
            }

            await _userService.SaveStep(new SaveStepRequest
            {
                ChatId = chatId,
                Phone = input,
                Step = "become_partner_step_final"
            });

            // Переход к следующему шагу
            var nextBuilder = _menuProvider.Value.GetBuilder("become_partner_step_final");
            return await nextBuilder.BuildMenu(chatId);
        }
    }
}
