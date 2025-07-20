using FloristAI.Core.Entities.UserInfo;

namespace FloristAI.Core.Store
{
    public interface ICacheRepository
    {
        Task<PartnerFormProgress?> GetProgress(long chatId);
        Task <bool> SaveProgress(PartnerFormProgress progress);
        Task <bool> ClearProgress(long chatId);
    }
}
