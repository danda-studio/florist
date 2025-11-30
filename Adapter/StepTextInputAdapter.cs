using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepFlowBuilder;
using FloristAI.Application.Users;


namespace FloristAI.Adapter
{
    public class StepTextInputAdapter : IMessageAdapter
    {
        private readonly IUserService _userService;
        private readonly IStepFlowService _stepFlowService;
        private readonly IStepFlowProvider _stepFlowProvider;

        public StepTextInputAdapter(IUserService userService, IStepFlowService stepFlowService, IStepFlowProvider stepFlowProvider)
        {
            _userService = userService;
            _stepFlowService = stepFlowService;
            _stepFlowProvider = stepFlowProvider;
        }

        public string RouteKey => "step_input";


        public async Task<List<MessageResult>> ProcessMessage(MessageContext context)
        {
            var userStep = await _stepFlowService.GetStep(context.ChatId);

            if(userStep == null || string.IsNullOrEmpty(userStep.Step))
            {
                return new List<MessageResult>
                {
                    new MessageResult
                    {
                        Text = "⚠️ Похоже, что вы не находитесь в процессе ввода данных. Пожалуйста, начните с шага меню."
                    }
                };
            }

            var stepBuilder = _stepFlowProvider.GetBuilder(userStep.Step);
            if (stepBuilder == null)
            {
                return new List<MessageResult>
                {
                    new MessageResult
                    {
                        Text = "🚫 Неизвестный шаг меню"
                    }
                };
            }

            var result = await stepBuilder.HandleInput(context.Message, context.ChatId);
            return result;
        }


    }
}
