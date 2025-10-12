using FloristAI.Application.Store;
using FloristAI.Application.Users.Models.Request;
using FloristAI.Application.Users.Models.Response;
using FloristAI.Core.Entities.UserInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application.Users
{
    public class StepFlowService : IStepFlowService
    {
        private readonly ICacheRepository _cacheRepository;
        public StepFlowService(ICacheRepository cacheRepository)
        {
            _cacheRepository = cacheRepository;
        }

        public async Task<GetStepResponse> GetStep(long chatId)
        {
            var step = await _cacheRepository.GetStepFlowBecomePartnerProgress(chatId);

            return new GetStepResponse
            {
                ChatId = chatId,
                Step = step.Step,
                FirstName = step.FirstName,
                LastName = step.LastName,
                Phone = step.Phone,
                Username = step.Username
            };

        }

        public async Task<bool> SaveStep(SaveStepRequest request)
        {
            if (request == null || request.ChatId <= 0)
            {
                throw new ArgumentException("Неверный запрос для сохранения шага");
            }

            // Получаем текущий прогресс
            var progress = await _cacheRepository.GetStepFlowBecomePartnerProgress(request.ChatId)
                           ?? new PartnerFormProgress { ChatId = request.ChatId };

            // Обновляем только то, что пришло в запросе
            if (request.Step != null)
                progress.Step = request.Step;

            if (!string.IsNullOrWhiteSpace(request.FirstName))
                progress.FirstName = request.FirstName;

            if (!string.IsNullOrWhiteSpace(request.LastName))
                progress.LastName = request.LastName;

            if (!string.IsNullOrWhiteSpace(request.Phone))
                progress.Phone = request.Phone;

            if (!string.IsNullOrWhiteSpace(request.TgUserName))
                progress.Username = request.TgUserName;

            // сохраняем обновлённый прогресс
            return await _cacheRepository.SaveStepFlowBecomePartnerProgress(progress);
        }

        public async Task<bool> ClearStep(long chatId)
        {
            if (chatId <= 0)
            {
                throw new ArgumentException("Неверный идентификатор чата");
            }
            return await _cacheRepository.ClearProgress(chatId);
        }
    }
}
