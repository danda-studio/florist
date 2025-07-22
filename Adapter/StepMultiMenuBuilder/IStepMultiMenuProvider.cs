using FloristAI.Adapter.StepMenuBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Adapter.StepMultiMenuBuilder
{
    public interface IStepMultiMenuProvider
    {
        public IStepMultiMenuBuilder GetBuilder(string step);
    }
}
