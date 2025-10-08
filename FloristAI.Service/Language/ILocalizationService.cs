
namespace FloristAI.Application.Language
{
    /// <summary>
    /// Интерфейс сервиса локализации для получения локализованных строк по ключу и коду языка.
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Получает локализованную строку по ключу и коду языка.
        /// </summary>
        /// <param name="key">Ключ локализованной строки (например, "Menu_Title").</param>
        /// <param name="languageCode">Код языка (например, "ru", "en", "ro").</param>
        /// <returns>Локализованная строка, если ключ найден, иначе возвращает сам ключ.</returns>
        string GetString(string key, string languageCode);

        string GetSheetName(string key);

        string GetFolderId(string key);

        string GetFolderName(string key);
    }
}
