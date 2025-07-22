using FloristAI.Adapter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Adapter.StepMultiMenuBuilder
{
    public interface IStepMultiMenuBuilder
    {
        string Step { get; }

        Task<List<MessageResult>> BuildMenu(long chatId);
    }
}
