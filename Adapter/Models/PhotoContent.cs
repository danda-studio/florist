
namespace FloristAI.Adapter.Models
{
    /// <summary>
    /// Модель фото
    /// </summary>
    public class PhotoContent
    {
        /// <summary>
        /// Ссылка на фото или file_id если TG
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Бинарные данные изображения (если нет URL)
        /// </summary>
        public byte[]? ImageBytes { get; set; }

        /// <summary>
        /// Описание фото
        /// </summary>
        public string? Description { get; set; }
    }
}
