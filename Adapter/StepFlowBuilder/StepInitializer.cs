using FloristAI.Application.Users;
using FloristAI.Application.Users.Models.Request;
using FloristAI.Application.Users.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Adapter.StepFlowBuilder
{
    public class StepInitializer : IStepInitializer
    {
        private readonly IUserService _userService;
        private readonly IStepFlowProvider _stepFlowProvider;

        public StepInitializer(IUserService userService, IStepFlowProvider stepFlowProvider)
        {
            _userService = userService;
            _stepFlowProvider = stepFlowProvider;
        }

        public async Task<PartnerFormProgress> EnsureStepInitialized(long chatId)
        {
            var progress = await _userService.GetStep(chatId);
            if (!string.IsNullOrEmpty(progress.Step))
            {
                return new PartnerFormProgress
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

            await _userService.SaveStep(request);

            return new PartnerFormProgress
            {
                ChatId = chatId,
                Step = entry.Step
            };
        }

    }

}
