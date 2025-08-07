using FloristAI.Core.Entities.UserInfo;

namespace FloristAI.Application.Store
{
    public interface ICacheRepository
    {
        Task<PartnerFormProgress> GetStepFlowBecomePartnerProgress(long chatId);
        Task<bool> SaveStepFlowBecomePartnerProgress(PartnerFormProgress progress);
        Task<bool> ClearProgress(long chatId);
    }
}
