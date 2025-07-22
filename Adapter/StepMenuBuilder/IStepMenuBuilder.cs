using FloristAI.Adapter.Models;

namespace FloristAI.Adapter.StepMenuBuilder
{
    public interface IStepMenuBuilder
    {
        string Step { get; }

        Task<List<MessageResult>> BuildMenu(long chatId);

    }
}
