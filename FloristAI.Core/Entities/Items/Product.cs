using FloristAI.Core.Entities.Enums;

namespace FloristAI.Core.Entities.Items
{
    /// <summary>
    /// Модель товара
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Уникальный идентификатор товара.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Тип товара (например, Цветок, Корзина, Букет).
        /// </summary>
        public ProductType ProductType { get; set; }

        /// <summary>
        /// Ссылка на изображение товара.
        /// </summary>
        public string? Image { get; set; } 

        /// <summary>
        /// Цена товара.
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// Список заказов
        /// </summary>
        public List<Order>? Orders { get; set; }

        /// <summary>
        /// Связанная сущность цветка
        /// </summary>
        public Flower? Flower { get; set; }

        /// <summary>
        /// Связанная сущность корзины
        /// </summary>
        public Basket? Basket { get; set; }

        /// <summary>
        /// Связанная сущность букета
        /// </summary>
        public Bouquet? Bouquet { get; set; }
    }
}
