using Adapter;
using Adapter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Router
{
    /// <summary>
    /// Роутер команд для Telegram-бота, перенаправляющий команды на соответствующие адаптеры.
    /// </summary>
    public class AdapterRouter
    {
        /// <summary>
        /// Словарь адаптеров
        /// </summary>
        private readonly Dictionary<string, IMessageAdapter> _adapters;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AdapterRouter"/>.
        /// </summary>
        /// <param name="adapters">Коллекция доступных адаптеров, реализующих интерфейс <see cref="IMessageAdapter"/>.</param>
        public AdapterRouter(IEnumerable<IMessageAdapter> adapters)
        {
            _adapters = adapters.ToDictionary(a => a.RouteKey); // "start" => StartAdapter
        }

        /// <summary>
        /// Обрабатывает входящее сообщение и перенаправляет его соответствующему адаптеру.
        /// </summary>
        /// <param name="message">Входящее сообщение от пользователя.</param>
        /// <param name="chatId">Идентификатор чата Telegram.</param>
        /// <returns>Результат обработки сообщения в виде объекта <see cref="MessageResult"/>.</returns>
        public async Task<MessageResult> RouteAsync(string message, long chatId)
        {
            string command = ExtractCommand(message); // "/start" -> "start"

            if (_adapters.TryGetValue(command, out var adapter))
            {
                return await adapter.ProcessMessage(message, chatId);
            }

            return new MessageResult { Text = "Неизвестная команда" };
        }

        /// <summary>
        /// Извлекает команду из текстового сообщения.
        /// </summary>
        /// <param name="message">Исходное сообщение, содержащее команду.</param>
        /// <returns>Имя команды без префикса '/'. Если сообщение пустое — возвращает пустую строку.</returns>
        private static string ExtractCommand(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return "";

            return message.Split(' ')[0].TrimStart('/').ToLower();
        }
    }
}
