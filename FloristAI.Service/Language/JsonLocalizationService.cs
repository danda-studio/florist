using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FloristAI.Application.Language
{
    /// <summary>
    /// Сервис локализации
    /// </summary>
    public class JsonLocalizationService : ILocalizationService
    {
        private readonly Dictionary<string, Dictionary<string, string>> _locales;
        private readonly string _localizationFolder;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="JsonLocalizationService"/>.
        /// </summary>
        /// <param name="env">Среда размещения приложения, используется для определения пути.</param>
        /// <param name="logger">Логгер для вывода сообщений о загрузке локализации.</param>
        public JsonLocalizationService(IHostEnvironment env, ILogger<JsonLocalizationService> logger)
        {
            _localizationFolder = Path.Combine(AppContext.BaseDirectory, "Localization");
            Console.WriteLine($"[i18n] Папка локализации: {_localizationFolder}");

            _locales = new Dictionary<string, Dictionary<string, string>>();

            // Загружаем доступные языки
            TryLoadLocale(logger, "ru");
            TryLoadLocale(logger, "ro");
        }

        /// <summary>
        /// Пытается загрузить локализационный файл для указанного языка.
        /// </summary>
        /// <param name="logger">Логгер для записи информации и ошибок.</param>
        /// <param name="lang">Код языка (например, "ru").</param>
        /// <returns>True, если файл успешно загружен, иначе false.</returns>
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

                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ошибка при загрузке локализации для языка {Language}", lang);
                return false;
            }
        }

        /// <summary>
        /// Получает локализованную строку по ключу и коду языка.
        /// </summary>
        /// <param name="key">Ключ строки (например, "Menu_Title").</param>
        /// <param name="languageCode">Код языка (например, "ru").</param>
        /// <returns>Локализованная строка, если найдена, иначе возвращается сам ключ.</returns>
        public string GetString(string key, string languageCode)
        {
            if (_locales.TryGetValue(languageCode, out var dict))
            {
                if (dict.TryGetValue(key, out var value))
                {
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
