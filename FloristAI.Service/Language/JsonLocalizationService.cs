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
            _localizationFolder = Path.Combine(env.ContentRootPath, "..", "FloristAI.Infrastructure", "Localization");
            _locales = new Dictionary<string, Dictionary<string, string>>();

            // Загружаем языки с обработкой ошибок
            TryLoadLocale(logger, "ru");
            TryLoadLocale(logger, "ro");
            TryLoadLocale(logger, "en"); // Английский как fallback
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
            // Сначала пробуем запрошенный язык
            if (_locales.TryGetValue(languageCode, out var dict) && dict.TryGetValue(key, out var value))
                return value;


            // В крайнем случае возвращаем ключ
            return key;
        }
    }
}