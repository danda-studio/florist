using FloristAI.Core.Entities.Enums;
using FloristAI.Core.Entities.Items;
using FloristAI.Core.Entities.UserInfo;

namespace FloristAI.Core.Entities
{
    /// <summary>
    /// Модель заказа
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Уникальный идентификатор заказа.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор продукта, включённого в заказ.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Идентификатор пользователя, оформившего заказ.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Текущий статус заказа.
        /// </summary>
        public OrderStatusType Status { get; set; }

        /// <summary>
        /// Пользователь, оформивший заказ.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Продукт, включённый в заказ.
        /// </summary>
        public Product Product { get; set; }
    }
}
