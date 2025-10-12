
namespace FloristAI.Application.Store
{
    public interface IGoogleDrive
    {
        Task<string> CreateFolder(string name, string parentFolderId);
        Task<(string Id, string Name)?> FindFolderByName(string name, string parentFolderId);
    }
}
