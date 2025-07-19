using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepFlowBuilder;
using FloristAI.Adapter.StepMenuBuilder;
using FloristAI.Application.Users;

namespace FloristAI.Adapter
{
    public class StepMessageAdapter : IMessageAdapter
    {
        public string RouteKey => "step_message";

        private readonly IStepInitializer _stepInitializer;
        private readonly IUserService _userService;
        private readonly IStepFlowProvider _menuProvider;

        public StepMessageAdapter(IStepInitializer stepInitializer, IStepFlowProvider menuProvider, IUserService userService)
        {
            _stepInitializer = stepInitializer;
            _menuProvider = menuProvider;
            _userService = userService;
        }

        public async Task<MessageResult> ProcessMessage(string messageText, long chatId)
        {
            var currentStep = await _userService.GetStep(chatId);

            if (string.IsNullOrWhiteSpace(currentStep.Step))
            {
                currentStep = await _stepInitializer.EnsureStepInitialized(chatId);
            }

            var builder = _menuProvider.GetBuilder(currentStep.Step);
            return await builder.HandleInput(messageText, chatId);
        }
    }

}

