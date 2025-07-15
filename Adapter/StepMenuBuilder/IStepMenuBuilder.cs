using FloristAI.Adapter.Models;

namespace FloristAI.Adapter.StepMenuBuilder
{
    public interface IStepMenuBuilder
    {
        string Step { get; }

        Task<MessageResult> BuildMenu(long chatId);

        Task<MessageResult> HandleInput(string input, long chatId);
    }
}
