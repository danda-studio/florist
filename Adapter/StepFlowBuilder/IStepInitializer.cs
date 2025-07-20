using FloristAI.Application.Users.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Adapter.StepFlowBuilder
{
    public interface IStepInitializer
    {
        Task<GetStepResponse> EnsureStepInitialized(long chatId);
    }

}
