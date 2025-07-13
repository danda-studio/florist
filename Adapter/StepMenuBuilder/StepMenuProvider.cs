using FloristAI.Adapter.RoleMenuBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Adapter.StepMenuBuilder
{
    public class StepMenuProvider : IStepMenuProvider
    {
        private readonly Dictionary<string, IStepMenuBuilder> _builders;

        public StepMenuProvider(IEnumerable<IStepMenuBuilder> builders)
        {
            _builders = builders.ToDictionary(b => b.Step, b => b);
            Console.WriteLine($"Зарегистрированные шаги: {string.Join(", ", _builders.Keys)}");
        }

        public IStepMenuBuilder GetBuilder(string step)
        {
            return _builders.TryGetValue(step, out var builder)
            ? builder
            : throw new InvalidOperationException($"No menu builder for step '{step}'");
        }
    }
}
