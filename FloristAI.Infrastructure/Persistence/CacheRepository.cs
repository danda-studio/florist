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
            var json = JsonConvert.SerializeObject(progress);
            return await _cache.StringSetAsync(GetKey(progress.ChatId), json, TimeSpan.FromHours(1));
        }



        public async Task<bool> ClearProgress(long chatId)
        {
            return await _cache.KeyDeleteAsync(GetKey(chatId));
        }
    }
}
