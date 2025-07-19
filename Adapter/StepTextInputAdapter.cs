using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepFlowBuilder;
using FloristAI.Application.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Adapter
{
    public class StepTextInputAdapter : IMessageAdapter
    {
        private readonly IUserService _userService;
        private readonly IStepFlowProvider _stepFlowProvider;


        public StepTextInputAdapter(IUserService userService, IStepFlowProvider stepFlowProvider)
        {
            _userService = userService;
            _stepFlowProvider = stepFlowProvider;
        }

        public string RouteKey => "step_input";


        public async Task<MessageResult> ProcessMessage(string text, long chatId)
        {
            var userStep = await _userService.GetStep(chatId);

            if(userStep == null || string.IsNullOrEmpty(userStep.Step))
            {
                return new MessageResult
                {
                    Text = "Пользователь не найден или шаг не установлен."
                };
            }

            var stepBuilder = _stepFlowProvider.GetBuilder(userStep.Step);
            if (stepBuilder == null)
            {
                return new MessageResult
                {
                    Text = "Неизвестный шаг меню."
                };
            }

            return await stepBuilder.HandleInput(text, chatId);
        }


    }
}
