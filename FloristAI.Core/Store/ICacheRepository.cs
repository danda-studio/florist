
using FloristAI.Core.Entities.UserInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Core.Store
{
    public interface ICacheRepository
    {
        Task<PartnerFormProgress?> GetProgress(long chatId);
        Task <bool> SaveProgress(PartnerFormProgress progress);
        Task <bool> ClearProgress(long chatId);
    }
}
