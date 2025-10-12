using FloristAI.Application.Users.Models.Request;
using FloristAI.Application.Users.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.Users
{
    public interface IStepFlowService
    {
        Task<GetStepResponse> GetStep(long chatId);
        Task<bool> SaveStep(SaveStepRequest request);
        Task<bool> ClearStep(long chatId);
    }
}
