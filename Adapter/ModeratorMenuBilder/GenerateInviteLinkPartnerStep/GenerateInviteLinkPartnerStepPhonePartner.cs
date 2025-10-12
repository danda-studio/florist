using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepFlowBuilder;
using FloristAI.Application.Language;
using FloristAI.Application.Users;
using FloristAI.Application.Users.Models.Request;
using FloristAI.Application.Validation;
using Telegram.Bot.Types.ReplyMarkups;

namespace FloristAI.Adapter.ModeratorMenuBilder.GenerateInviteLinkPartnerStep
{
    public class GenerateInviteLinkPartnerStepPhonePartner : IStepFlowBuilder
    {
        private readonly IUserService _userService;
        private readonly IStepFlowService _stepFlowService;

        private readonly ILocalizationService _localizationService;

        private readonly Lazy<IStepFlowProvider> _menuProvider;
        public GenerateInviteLinkPartnerStepPhonePartner(IUserService userService, IStepFlowService stepFlowService, ILocalizationService localizationService, Lazy<IStepFlowProvider> menuProvider)
        {
            _userService = userService;
            _stepFlowService = stepFlowService;
            _localizationService = localizationService;
            _menuProvider = menuProvider;
        }

        public string Step => "generate_partnerLink_phone_moderator";

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
                new[] { InlineKeyboardButton.WithCallbackData(_localizationService.GetString("Button_Back", user.LanguageCode), "step_message:generate_partnerLink_lastName_moderator") },
            };
            await _stepFlowService.SaveStep(new SaveStepRequest
            {
                ChatId = chatId,
                Step = Step
            });
            return new List<MessageResult>
            {
                new MessageResult
                {
                    Text = _localizationService.GetString("Generate_PartnerLink_Phone", user.LanguageCode),
                    ReplyMarkup = keyboard
                }
            };
        }

        public async Task<List<MessageResult>> HandleInput(string input, long chatId)
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

            if (!Validator.IsValidPhone(input))
            {
                return new List<MessageResult>
                {
                    new MessageResult
                    {
                        Text = _localizationService.GetString("Phone_Validation_Error", user.LanguageCode),
                        ReplyMarkup = null
                    }
                };
            }

            await _stepFlowService.SaveStep(new SaveStepRequest
            {
                ChatId = chatId,
                Phone = input,
                Step = "generate_partnerLink_final_moderator"
            });

            var nextBuilder = _menuProvider.Value.GetBuilder("generate_partnerLink_final_moderator");
            return await nextBuilder.BuildMenu(chatId);
        }
    }
}
