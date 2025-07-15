using FloristAI.Adapter.Models;

namespace FloristAI.Adapter.RoleMenuBuilder
{
    public interface IRoleMenuBuilder
    {
        string Role { get; }

        Task<MessageResult> BuildMenu(long chatId);
    }
}
