using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Adapter.RoleMenuBuilder
{
    public class RoleMenuBuilderProvider : IRoleMenuBuilderProvider
    {
        private readonly Dictionary<string, IRoleMenuBuilder> _builders;

        public RoleMenuBuilderProvider(IEnumerable<IRoleMenuBuilder> builders)
        {
            _builders = builders.ToDictionary(b => b.Role, b => b);
            Console.WriteLine($"Зарегистрированные роли: {string.Join(", ", _builders.Keys)}");
        }

        public IRoleMenuBuilder GetBuilder(string role)
        {
            return _builders.TryGetValue(role, out var builder)
            ? builder
            : throw new InvalidOperationException($"No menu builder for role '{role}'");
        }
    }
}
