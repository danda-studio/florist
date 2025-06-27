using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FloristAI.Application.Language
{
    public class JsonLocalizationService : ILocalizationService
    {
        private readonly Dictionary<string, Dictionary<string, string>> _locales;

        public JsonLocalizationService()
        {
            _locales = new Dictionary<string, Dictionary<string, string>>();
            LoadLocale("ru");
            LoadLocale("ro");
        }

        private void LoadLocale(string lang)
        {
            var json = File.ReadAllText($"{lang}.json"); // путь к файлам с локализацией
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            _locales[lang] = dict ?? new Dictionary<string, string>();
        }

        public string GetString(string key, string languageCode)
        {
            if (_locales.TryGetValue(languageCode, out var dict))
            {
                if (dict.TryGetValue(key, out var value))
                    return value;
            }
            return key; 
        }
    }

}
