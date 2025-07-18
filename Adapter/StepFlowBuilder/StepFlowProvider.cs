using FloristAI.Adapter.StepMenuBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Adapter.StepFlowBuilder
{
    public class StepFlowProvider : IStepFlowProvider
    {
        private readonly Dictionary<string, IStepFlowBuilder> _builders;

        public StepFlowProvider(IEnumerable<IStepFlowBuilder> builders)
        {
            _builders = builders.ToDictionary(b => b.Step, b => b);
            Console.WriteLine($"Зарегистрированные шаги: {string.Join(", ", _builders.Keys)}");
        }

        public IStepFlowBuilder GetBuilder(string step)
        {
            return _builders.TryGetValue(step, out var builder)
            ? builder
            : throw new InvalidOperationException($"No menu builder for step '{step}'");
        }
    }
}
