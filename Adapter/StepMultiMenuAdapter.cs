using FloristAI.Adapter.Models;
using FloristAI.Adapter.StepMultiMenuBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Adapter
{
    public class StepMultiMenuAdapter : IMessageAdapter
    {
        public string RouteKey => "step_multi";
        private readonly IStepMultiMenuProvider _stepMultiMenuProvider;
        public StepMultiMenuAdapter(IStepMultiMenuProvider stepMultiMenuProvider)
        {
            _stepMultiMenuProvider = stepMultiMenuProvider;
        }
        public async Task<List<MessageResult>> ProcessMessage(string parameter, long chatId)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                return new List<MessageResult> { new MessageResult { Text = "Параметр не может быть пустым." } };
            }

            var builder = _stepMultiMenuProvider.GetBuilder(parameter);

            if (builder == null)
            {
                return new List<MessageResult> { new MessageResult { Text = "Неизвестный шаг меню." } };
            }

            return await builder.BuildMenu(chatId); 
        }

    }
}
