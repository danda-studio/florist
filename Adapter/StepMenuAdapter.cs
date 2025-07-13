using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepMenuBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<MessageResult> ProcessMessage(string parameter, long chatId)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                return new MessageResult { Text = "Параметр не может быть пустым." };
            }
            var builder = _stepMenuProvider.GetBuilder(parameter);
            if (builder == null)
            {
                return new MessageResult { Text = "Неизвестный шаг меню." };
            }
            return await builder.BuildMenu(chatId);
        }
    }
}
