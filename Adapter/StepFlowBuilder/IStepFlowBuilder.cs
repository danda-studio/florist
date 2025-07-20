using FloristAI.Adapter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Adapter.StepFlowBuilder
{
    public interface IStepFlowBuilder
    {
        string Step { get; }

        bool IsEntryPoint => false;

        Task<MessageResult> BuildMenu(long chatId);

        Task<MessageResult> HandleInput(string input, long chatId);
    }
}
