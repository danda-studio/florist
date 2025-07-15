using FloristAI.Application.Language.Models;

namespace FloristAI.Application.Language
{
    /// <summary>
    /// Интерфейс сервиса языков
    /// </summary>
    public interface ILanguageService
    {
        /// <summary>
        /// Метод для получения списка языков
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<LanguageModel>> GetLanguageList(long chatId);
    }
}
