using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepMenuBuilder;
using FloristAI.Application.Users;
using FloristAI.Application.Users.Models.Request;
using FloristAI.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Adapter
{
    public class StepMessageAdapter : IMessageAdapter
    {
        public string RouteKey => "step_message"; 

        private readonly IUserService _userService;
        private readonly IStepMenuProvider _menuProvider;

        public StepMessageAdapter(IUserService userService, IStepMenuProvider menuProvider)
        {
            _userService = userService;
            _menuProvider = menuProvider;
        }

        public async Task<MessageResult> ProcessMessage(string messageText, long chatId)
        {
            var progress = await _userService.GetStep(chatId); // получаем текущий шаг

            switch (progress.Step)
            {
                case PartnerFormStep.EnterName:
                    await _userService.SaveStep(new SaveStepRequest
                    {
                        ChatId = chatId,
                        FirstName = messageText,
                        Step = PartnerFormStep.EnterSurname // переходим к следующему шагу
                    });

                    var nextMenu = _menuProvider.GetBuilder("become_partner_step_lastName");
                    return await nextMenu.BuildMenu(chatId);

                case PartnerFormStep.EnterSurname:
                    await _userService.SaveStep(new SaveStepRequest
                    {
                        ChatId = chatId,
                        LastName = messageText,
                        Step = PartnerFormStep.EnterPhone
                    });

                    var phoneMenu = _menuProvider.GetBuilder("become_partner_step_phone");
                    return await phoneMenu.BuildMenu(chatId);

                case PartnerFormStep.EnterPhone:
                    await _userService.SaveStep(new SaveStepRequest
                    {
                        ChatId = chatId,
                        Phone = messageText,
                        Step = PartnerFormStep.Completed
                    });

                    return new MessageResult
                    {
                        Text = "Спасибо, вы прошли все шаги!",
                        ReplyMarkup = null
                    };

                default:
                    return new MessageResult
                    {
                        Text = "Неподдерживаемый шаг. Попробуйте снова.",
                        ReplyMarkup = null
                    };
            }
        }
    }

}
