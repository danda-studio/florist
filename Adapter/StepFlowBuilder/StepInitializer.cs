using FloristAI.Application.Users;
using FloristAI.Application.Users.Models.Request;
using FloristAI.Application.Users.Models.Response;


namespace FloristAI.Adapter.StepFlowBuilder
{
    public class StepInitializer : IStepInitializer
    {
        private readonly IUserService _userService;
        private readonly IStepFlowService _stepFlowService;
        private readonly IStepFlowProvider _stepFlowProvider;

        public StepInitializer(IUserService userService, IStepFlowService stepFlowService, IStepFlowProvider stepFlowProvider)
        {
            _userService = userService;
            _stepFlowService = stepFlowService;
            _stepFlowProvider = stepFlowProvider;
        }

        public async Task<GetStepResponse> EnsureStepInitialized(long chatId)
        {
            var progress = await _stepFlowService.GetStep(chatId);
            if (!string.IsNullOrEmpty(progress.Step))
            {
                return new GetStepResponse
                {
                    ChatId = progress.ChatId,
                    Step = progress.Step,
                    FirstName = progress.FirstName,
                    LastName = progress.LastName,
                    Phone = progress.Phone
                };
            }

            var entry = _stepFlowProvider.GetEntryPointBuilder();

            // Сохраняем первый шаг
            var request = new SaveStepRequest
            {
                ChatId = chatId,
                Step = entry.Step
            };

            await _stepFlowService.SaveStep(request);

            return new GetStepResponse
            {
                ChatId = chatId,
                Step = entry.Step
            };
        }

    }

}
