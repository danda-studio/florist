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
        private readonly IStepFlowBuilder _entryPointBuilder;

        public StepFlowProvider(IEnumerable<IStepFlowBuilder> builders)
        {
            _builders = builders.ToDictionary(b => b.Step, b => b);
            _entryPointBuilder = builders.FirstOrDefault(b => b.IsEntryPoint)
            ?? throw new InvalidOperationException("No entry point defined");
            Console.WriteLine($"Зарегистрированные шаги: {string.Join(", ", _builders.Keys)}");
        }

        public IStepFlowBuilder GetBuilder(string step)
        {
            return _builders.TryGetValue(step, out var builder)
            ? builder
            : throw new InvalidOperationException($"No menu builder for step '{step}'");
        }

        public IStepFlowBuilder GetEntryPointBuilder() => _entryPointBuilder;
    }
}
