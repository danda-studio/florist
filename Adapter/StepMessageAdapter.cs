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

        public async Task<List<MessageResult>> ProcessMessage(MessageContext context)
        {
            var builder = _menuProvider.GetBuilder(context.Message);
            if (builder == null)
            {
                return new List<MessageResult>
                {
                    new MessageResult
                    {
                        Text = $"Step '{context.Message}' not found."
                    }
                };
            }

            var result = await builder.BuildMenu(context.ChatId);
            return result;

        }
    }

}

