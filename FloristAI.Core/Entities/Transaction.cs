using FloristAI.Core.Entities.Enums;
using FloristAI.Core.Entities.UserInfo;

namespace FloristAI.Core.Entities
{
    /// <summary>
    /// Модель транзакции оплаты
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Уникальный идентификатор транзакции.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя, связанного с данной транзакцией.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Статус текущей транзакции.
        /// </summary>
        public TransactionStatusType Status { get; set; }

        /// <summary>
        /// Пользователь, связанный с данной транзакцией.
        /// </summary>
        public User User { get; set; }
    }
}
