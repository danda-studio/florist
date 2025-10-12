using FloristAI.Application.Language.Models;
using FloristAI.Application.Store;
using FloristAI.Core.Entities.Enums;

namespace FloristAI.Application.Language
{
    /// <summary>
    /// Сервис язык интерфейса
    /// </summary>
    public class LanguageService : ILanguageService
    {
        /// <summary>
        /// Репозиторий для работы с данными пользователей.
        /// </summary>
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="LanguageService"/>.
        /// </summary>
        /// <param name="userRepository">Репозиторий пользователей.</param>
        public LanguageService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Метод для получения языков
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<LanguageModel>> GetLanguageList(long chatId)
        {

            var languages = Enum.GetValues(typeof(LanguageType))
                .Cast<LanguageType>()
                .Select(lang => new LanguageModel
                {
                    Id = (int)lang,
                    Name = GetLanguageName(lang),
                    Code = GetLanguageCode(lang)
                });
            return await Task.FromResult(languages);
        }

        /// <summary>
        /// Метод для преобразавания enum-значения в текст
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        private static string GetLanguageName(LanguageType lang) => lang switch
        {
            LanguageType.Russian => "🇷🇺 Русский",
            LanguageType.Romanian => "🇷🇴 Română",
            _ => "🇷🇺 Русский"
        };

        /// <summary>
        /// Метод для получения кода языка интерфейса
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        private static string GetLanguageCode(LanguageType lang) => lang switch
        {
            LanguageType.Russian => "ru",
            LanguageType.Romanian => "ro",
            _ => "ru"
        };
    }
}
