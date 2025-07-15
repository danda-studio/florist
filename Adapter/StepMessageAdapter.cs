using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepMenuBuilder;
using FloristAI.Application.Users;

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
            var progress = await _userService.GetStep(chatId);

            var builder = _menuProvider.GetBuilder(progress.Step);
            return await builder.HandleInput(messageText, chatId);
        }

    }
}

