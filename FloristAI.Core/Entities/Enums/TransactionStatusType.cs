using System;

namespace FloristAI.Core.Entities.Enums
{
    /// <summary>
    /// Перечисление статусов транзакции.
    /// </summary>
    public enum TransactionStatusType
    {
        /// <summary>
        /// Транзакция обработана.
        /// </summary>
        Processed = 0,

        /// <summary>
        /// Транзакция прошла успешно.
        /// </summary>
        Passed = 1,

        /// <summary>
        /// Транзакция отменена.
        /// </summary>
        Canceled = 2,
    }
}
