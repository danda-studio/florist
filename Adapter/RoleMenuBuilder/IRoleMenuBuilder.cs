using FloristAI.Adapter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Adapter.RoleMenuBuilder
{
    public interface IRoleMenuBuilder
    {
        string Role { get; }

        Task<MessageResult> BuildMenu(long chatId);
    }
}
