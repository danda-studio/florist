using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FloristAI.Application.Language
{
    public class JsonLocalizationService : ILocalizationService
    {
        private readonly Dictionary<string, Dictionary<string, string>> _locales;
        private readonly string _localizationFolder;

        public JsonLocalizationService(IHostEnvironment env, ILogger<JsonLocalizationService> logger)
        {
            _localizationFolder = Path.Combine(AppContext.BaseDirectory, "Localization");
            Console.WriteLine($"[i18n] Папка локализации: {_localizationFolder}");
            _locales = new Dictionary<string, Dictionary<string, string>>();

            // Загружаем языки с обработкой ошибок
            TryLoadLocale(logger, "ru");
            TryLoadLocale(logger, "ro");
        }

        private bool TryLoadLocale(ILogger logger, string lang)
        {
            try
            {
                var filePath = Path.Combine(_localizationFolder, $"{lang}.json");

                if (!File.Exists(filePath))
                {
                    logger.LogWarning("Файл локализации не найден: {FilePath}", filePath);
                    return false;
                }

                var json = File.ReadAllText(filePath);
                _locales[lang] = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                                  ?? new Dictionary<string, string>();

                logger.LogInformation("Успешно загружена локализация для языка {Language}", lang);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при загрузке локализации для языка {Language}", lang);
                return false;
            }
        }

        public string GetString(string key, string languageCode)
        {

            if (_locales.TryGetValue(languageCode, out var dict))
            {
                if (dict.TryGetValue(key, out var value))
                {
                    Console.WriteLine($"[i18n] Нашли: [{languageCode}] {key} => {value}");
                    return value;
                }
                else
                {
                    Console.WriteLine($"[i18n] Ключ не найден: {key} в языке {languageCode}");
                }
            }
            else
            {
                Console.WriteLine($"[i18n] Язык не найден: {languageCode}");
            }

            return key;
        }

    }
}