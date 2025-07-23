using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepFlowBuilder;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using FloristAI.Application.Users.Models.Request;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.ClientMenuBuilder.BecomePartnerStep
{
    public class BecomePartnerStepLastName : IStepFlowBuilder
    {
        private readonly IUserService _userService;

        private readonly ILocalizationService _localizationService;

        private readonly Lazy<IStepFlowProvider> _menuProvider;
        public BecomePartnerStepLastName(IUserService userService, ILocalizationService localizationService, Lazy<IStepFlowProvider> menuProvider)
        {
            _userService = userService;
            _localizationService = localizationService;
            _menuProvider = menuProvider;
        }

        public string Step => "become_partner_step_lastName";

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

            var keyboard = new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Button_Back", user.LanguageCode), "step_message:become_partner_step_firstName") },
            };
            return new List<MessageResult>
            {
                new MessageResult
                {
                    Text = _localizationService.GetString("Become_Input_LastName", user.LanguageCode),
                    ReplyMarkup = keyboard
                }
            };
        }

        public async Task<List<MessageResult>> HandleInput(string input, long chatId)
        {
            await _userService.SaveStep(new SaveStepRequest
            {
                ChatId = chatId,
                LastName = input,
                Step = "become_partner_step_phone"
            });

            var nextBuilder = _menuProvider.Value.GetBuilder("become_partner_step_phone");
            return await nextBuilder.BuildMenu(chatId);
        }
    }
}
