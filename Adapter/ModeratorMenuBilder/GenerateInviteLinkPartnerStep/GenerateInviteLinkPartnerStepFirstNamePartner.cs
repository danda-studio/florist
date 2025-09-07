using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepFlowBuilder;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using FloristAI.Application.Users.Models.Request;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.ModeratorMenuBilder.GenerateInviteLinkPartnerStep
{
    public class GenerateInviteLinkPartnerStepFirstNamePartner : IStepFlowBuilder
    {
        private readonly IUserService _userService;

        private readonly ILocalizationService _localizationService;

        private readonly Lazy<IStepFlowProvider> _menuProvider;
        public GenerateInviteLinkPartnerStepFirstNamePartner(IUserService userService, ILocalizationService localizationService, Lazy<IStepFlowProvider> menuProvider)
        {
            _userService = userService;
            _localizationService = localizationService;
            _menuProvider = menuProvider;
        }

        public string Step => "generate_partnerlink_firstname_moderator";

        public bool IsEntryPoint => true;

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

            await _userService.SaveStep(new SaveStepRequest
            {
                ChatId = chatId,
                Step = Step,
                LastMessageId = null,
                TgUserName = username

            });

            var messages = new List<MessageResult>();

            var keyboard = new[]
            {
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Button_Back", user.LanguageCode), "role_menu:Moderator") },
            };

            messages.Add(new MessageResult
            {
                Text = _localizationService.GetString("Generator_Url_Title", user.LanguageCode),
                ReplyMarkup = null,
                PinnedMessage = true
            });
            ModeratorMenuBuilder._hasTempPin.Add(chatId);
            messages.Add(new MessageResult
            {
                Text = _localizationService.GetString("Generate_PartnerLink_FirstName", user.LanguageCode),
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
                Step = "generate_partnerLink_lastName_moderator",
            });

            var nextBuilder = _menuProvider.Value.GetBuilder("generate_partnerLink_lastName_moderator");

            return await nextBuilder.BuildMenu(chatId);
        }

    }
}

