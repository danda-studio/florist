using System;

namespace FloristAI.Core.Entities.Items
{
    /// <summary>
    /// Модель корзинки
    /// </summary>
    public class Basket
    {
        /// <summary>
        /// Уникальный идентификатор корзины.
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
