using System;

namespace FloristAI.Core.Entities.Items
{
    /// <summary>
    /// Модель цветка
    /// </summary>
    public class Flower
    {
        /// <summary>
        /// Уникальный идентификатор цветка.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор связанного товара.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Название цветка.
        /// </summary>
        public string? Name { get; set; } 

        /// <summary>
        /// Цвет цветка.
        /// </summary>
        public string? Color { get; set; } 

        /// <summary>
        /// Размер цветка (например, в сантиметрах).
        /// </summary>
        public int? Size { get; set; } 

        /// <summary>
        /// Порядковый номер цветка для отображения.
        /// </summary>
        public int OrdinalNumber { get; set; }

        /// <summary>
        /// Связанная сущность товара
        /// </summary>
        public Product Product { get; set; }
    }
}
