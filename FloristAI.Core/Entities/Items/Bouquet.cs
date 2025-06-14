using System;

namespace FloristAI.Core.Entities.Items
{
    /// <summary>
    /// Модель букета
    /// </summary>
    public class Bouquet
    {
        /// <summary>
        /// Уникальный идентификатор букета.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор связанного товара.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Связанный сущность товара
        /// </summary>
        public Product Product { get; set; }
    }
}
