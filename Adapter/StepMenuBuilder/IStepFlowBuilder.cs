using FloristAI.Adapter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Adapter.StepMenuBuilder
{
    public interface IStepFlowBuilder
    {
        public string Step { get; }

        public Task<MessageResult> BuildMenu(string input, long chatId);
    }
}
