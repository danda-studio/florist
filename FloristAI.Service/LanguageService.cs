using FloristAI.Application.Models.Response;
using FloristAI.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Application
{
    /// <summary>
    /// Сервис язык интерфейса
    /// </summary>
    public class LanguageService : ILanguageService
    {
        /// <summary>
        /// Метод для получения языков
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Language>> GetLanguageList ()
        {
            var languages = Enum.GetValues(typeof(LanguageType))
                .Cast<LanguageType>()
                .Select(lang => new Language
                {
                    Id = (int)lang,
                    Name = GetLanguageName(lang),
                    Code = GetLanguageCode(lang)
                });
            return Task.FromResult(languages);
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
