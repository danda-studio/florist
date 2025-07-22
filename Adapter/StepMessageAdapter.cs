using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepFlowBuilder;
using FloristAI.Adapter.StepMenuBuilder;
using FloristAI.Application.Users;

namespace FloristAI.Adapter
{
    public class StepMessageAdapter : IMessageAdapter
    {
        public string RouteKey => "step_message";

        private readonly IStepFlowProvider _menuProvider;

        public StepMessageAdapter(IStepFlowProvider menuProvider)
        {
            _menuProvider = menuProvider;
        }

        public async Task<List<MessageResult>> ProcessMessage(string stepName, long chatId)
        {
            var builder = _menuProvider.GetBuilder(stepName);
            if (builder == null)
            {
                return new List<MessageResult>
                {
                    new MessageResult
                    {
                        Text = $"Step '{stepName}' not found."
                    }
                };
            }

            var result = await builder.BuildMenu(chatId);
            return new List<MessageResult> { result };

        }
    }

}

