using FloristAI.Adapter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloristAI.Adapter
{
    /// <summary>
    /// Интерфейс адаптера для обработки сообщений в зависимости от команды.
    /// </summary>
    public interface IMessageAdapter
    {
        /// <summary>
        /// Ключ маршрута (команда), с которым ассоциирован адаптер.
        /// Пример: "start", "select_language".
        /// </summary>
        string RouteKey { get; }

        /// <summary>
        /// Обрабатывает входящее сообщение и возвращает результат обработки.
        /// </summary>
        /// <param name="message">Сообщение от пользователя, включая команду и аргументы.</param>
        /// <param name="chatId">Идентификатор чата, откуда пришло сообщение.</param>
        /// <returns>Результат обработки сообщения в виде объекта <see cref="MessageResult"/>.</returns>
        Task<MessageResult> ProcessMessage(string message, long chatId);
    }
}
