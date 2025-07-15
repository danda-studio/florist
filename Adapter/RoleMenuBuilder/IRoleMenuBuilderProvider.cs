
namespace FloristAI.Adapter.RoleMenuBuilder
{
    public interface IRoleMenuBuilderProvider
    {
        public IRoleMenuBuilder GetBuilder(string role);
    }
}
