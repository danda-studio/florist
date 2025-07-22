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

        public async Task<List<MessageResult>> ProcessMessage(string parameter, long chatId)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                return new List<MessageResult> { new MessageResult { Text = "Параметр не может быть пустым." } };
            }

            var builder = _stepMenuProvider.GetBuilder(parameter);
            if (builder == null)
            {
                return new List<MessageResult> { new MessageResult { Text = "Неизвестный шаг меню." } };
            }

            var result = await builder.BuildMenu(chatId); // уже List<MessageResult>
            return result;
        }
    }
}
