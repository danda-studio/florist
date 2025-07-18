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
        private readonly IStepFlowProvider _menuProvider;

        public StepMessageAdapter(IStepInitializer stepInitializer, IStepFlowProvider menuProvider)
        {
            _stepInitializer = stepInitializer;
            _menuProvider = menuProvider;
        }

        public async Task<MessageResult> ProcessMessage(string messageText, long chatId)
        {
            var progress = await _stepInitializer.EnsureStepInitialized(chatId);

            var builder = _menuProvider.GetBuilder(progress.Step);
            return await builder.HandleInput(messageText, chatId);
        }
    }

}

