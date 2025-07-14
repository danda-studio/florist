using FloristAI.Adapter.RoleMenuBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Adapter.StepMenuBuilder
{
    public interface IStepMenuProvider
    {
        public IStepMenuBuilder GetBuilder(string step);
    }
}
