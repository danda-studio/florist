using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepMenuBuilder;

namespace FloristAI.Adapter
{
    public class StepMenuAdapter : IMessageAdapter
    {
        public string RouteKey => "step";

        private readonly IStepMenuProvider _stepMenuProvider;

        public StepMenuAdapter(IStepMenuProvider stepMenuProvider)
        {
            _stepMenuProvider = stepMenuProvider;
        }

        public async Task<List<MessageResult>> ProcessMessage(MessageContext context)
        {

            var builder = _stepMenuProvider.GetBuilder(context.Message);
            if (builder == null)
            {
                return new List<MessageResult> { new MessageResult { Text = "Неизвестный шаг меню." } };
            }

            var result = await builder.BuildMenu(context.ChatId); 
            return result;
        }
    }
}
