using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepFlowBuilder;
using FloristAI.Adapter.StepMenuBuilder;
using FloristAI.Application.Users;

namespace FloristAI.Adapter
{
    public class StepMessageAdapter : IMessageAdapter
    {
        public string RouteKey => "step_message"; 

        private readonly IUserService _userService;
        private readonly IStepFlowProvider _menuProvider;

        public StepMessageAdapter(IUserService userService, IStepFlowProvider menuProvider)
        {
            _userService = userService;
            _menuProvider = menuProvider;
        }

        public async Task<MessageResult> ProcessMessage(string messageText, long chatId)
        {
            var progress = await _userService.GetStep(chatId);

            var builder = _menuProvider.GetBuilder(progress.Step.ToString());
            return await builder.HandleInput(messageText, chatId);
        }

    }
}

