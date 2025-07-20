
namespace FloristAI.Adapter.StepMenuBuilder
{
    public interface IStepMenuProvider
    {
        public IStepMenuBuilder GetBuilder(string step);
    }
}
