using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Adapter.RoleMenuBuilder
{
    public interface IRoleMenuBuilderProvider
    {
        public IRoleMenuBuilder GetBuilder(string role, long? chatId);
    }
}
