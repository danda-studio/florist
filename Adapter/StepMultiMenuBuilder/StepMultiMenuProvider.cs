using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Adapter.StepMultiMenuBuilder
{
    public class StepMultiMenuProvider : IStepMultiMenuProvider
    {
        private readonly Dictionary<string, IStepMultiMenuBuilder> _builders;
        public StepMultiMenuProvider(IEnumerable<IStepMultiMenuBuilder> builders)
        {
            _builders = builders.ToDictionary(b => b.Step, b => b);
            Console.WriteLine($"Зарегистрированные шаги: {string.Join(", ", _builders.Keys)}");
        }
        public IStepMultiMenuBuilder GetBuilder(string step)
        {
            return _builders.TryGetValue(step, out var builder)
                ? builder
                : throw new InvalidOperationException($"No menu builder for step '{step}'");
        }

    }
}
