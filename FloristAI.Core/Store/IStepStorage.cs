using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Core.Store
{
    public interface IStepStorage
    {
        Task<string?> GetStep(long chatId);
        Task SetStep(long chatId, string step);
        Task SaveValue(long chatId, string key, string value);
        Task<string?> GetValue(long chatId, string key);
        Task Clear(long chatId);
    }

}
