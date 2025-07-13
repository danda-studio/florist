using FloristAI.Adapter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Adapter.StepMenuBuilder
{
    public interface IStepMenuBuilder
    {
        string Step { get; }

        Task<MessageResult> BuildMenu(long chatId);
    }
}
