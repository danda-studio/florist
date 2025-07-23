using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepFlowBuilder;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using FloristAI.Application.Users.Models.Request;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.ClientMenuBuilder.BecomePartnerStep
{
    public class BecomePartnerStepFirstName : IStepFlowBuilder
    {
        private readonly IUserService _userService;
        
        private readonly ILocalizationService _localizationService;

        private readonly Lazy<IStepFlowProvider> _menuProvider;
        public BecomePartnerStepFirstName(IUserService userService, ILocalizationService localizationService, Lazy<IStepFlowProvider> menuProvider)
        {
            _userService = userService;
            _localizationService = localizationService;
            _menuProvider = menuProvider;
        }

        public string Step => "become_partner_step_firstName";

        public bool IsEntryPoint => true;

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

            await _userService.SaveStep(new SaveStepRequest
            {
                ChatId = chatId,
                Step = Step,
                LastMessageId = null
            });

            var messages = new List<MessageResult>();

            var keyboard = new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Button_Back", user.LanguageCode), "role_menu:Client") },
            };

            messages.Add(new MessageResult
            {
                Text = _localizationService.GetString("Become_Partner_Title", user.LanguageCode),
                ReplyMarkup = null,
                PinnedMessage = true 
            });
            messages.Add(new MessageResult
            {
                Text = _localizationService.GetString("Become_Input_FirstName", user.LanguageCode),
                ReplyMarkup = keyboard
            });

            return messages;
        }

        public async Task<List<MessageResult>> HandleInput(string input, long chatId)
        {
            await _userService.SaveStep(new SaveStepRequest
            {
                ChatId = chatId,
                FirstName = input,
                Step = "become_partner_step_lastName",
            });

            var nextBuilder = _menuProvider.Value.GetBuilder("become_partner_step_lastName");
            
            return await nextBuilder.BuildMenu(chatId);
        }

    }
}
