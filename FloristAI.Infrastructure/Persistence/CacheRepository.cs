using FloristAI.Core.Entities.UserInfo;
using FloristAI.Core.Store;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace FloristAI.Infrastructure.Persistence
{
    public class CacheRepository : ICacheRepository
    {
        private readonly IDatabase _cache;

        public CacheRepository(IConnectionMultiplexer connectionMultiplexer)
        {
            _cache = connectionMultiplexer.GetDatabase();
        }

        private string GetKey(long chatId) => $"partner_form:{chatId}";

        public async Task<PartnerFormProgress?> GetProgress(long chatId)
        {
            var json = await _cache.StringGetAsync(GetKey(chatId));
            return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<PartnerFormProgress>(json);
        }

        public async Task<bool> SaveProgress(PartnerFormProgress progress)
        {
            var key = GetKey(progress.ChatId);

            // Получаем текущий прогресс, если есть
            var existingJson = await _cache.StringGetAsync(key);
            PartnerFormProgress? existingProgress = string.IsNullOrEmpty(existingJson)
                ? new PartnerFormProgress { ChatId = progress.ChatId }
                : JsonConvert.DeserializeObject<PartnerFormProgress>(existingJson);

            // Обновляем только заполненные поля (можно гибко расширить)
            if (!string.IsNullOrEmpty(progress.FirstName))
                existingProgress.FirstName = progress.FirstName;

            if (!string.IsNullOrEmpty(progress.LastName))
                existingProgress.LastName = progress.LastName;

            if (!string.IsNullOrEmpty(progress.Phone))
                existingProgress.Phone = progress.Phone;

            // и т.д. — добавь остальные поля, если есть

            // Сохраняем обратно
            var updatedJson = JsonConvert.SerializeObject(existingProgress);
            return await _cache.StringSetAsync(key, updatedJson, TimeSpan.FromHours(1));
        }


        public async Task<bool> ClearProgress(long chatId)
        {
            return await _cache.KeyDeleteAsync(GetKey(chatId));
        }
    }
}
