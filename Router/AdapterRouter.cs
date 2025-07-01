using FloristAI.Adapter;
using FloristAI.Adapter.Models;
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
        /// Словарь адаптеров, где ключ — RouteKey, а значение — адаптер.
        /// </summary>
        private readonly Dictionary<string, IMessageAdapter> _adapters;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AdapterRouter"/>.
        /// </summary>
        /// <param name="adapters">Коллекция адаптеров, реализующих <see cref="IMessageAdapter"/>.</param>
        public AdapterRouter(IEnumerable<IMessageAdapter> adapters)
        {
            _adapters = adapters.ToDictionary(a => a.RouteKey);
            Console.WriteLine($"Зарегистрированные ключи: {string.Join(", ", _adapters.Keys)}");
        }

        /// <summary>
        /// Обрабатывает входящее сообщение или callback, направляя на соответствующий адаптер.
        /// </summary>
        /// <param name="message">Входящее сообщение или callback-данные.</param>
        /// <param name="chatId">ID чата Telegram.</param>
        /// <returns>Результат обработки в виде <see cref="MessageResult"/>.</returns>
        public async Task<MessageResult> RouteAsync(string message, long chatId)
        {
            if (string.IsNullOrWhiteSpace(message))
                return new MessageResult { Text = "Неизвестная команда" };

            if (message.Contains(":"))
                return await RouteCallbackAsync(message, chatId);
            else
                return await RouteCommandAsync(message, chatId);
        }

        /// <summary>
        /// Обрабатывает команду (текстовое сообщение без параметров).
        /// </summary>
        /// <param name="message">Команда (например, "/start").</param>
        /// <param name="chatId">ID чата Telegram.</param>
        /// <returns>Результат обработки команды.</returns>
        private async Task<MessageResult> RouteCommandAsync(string message, long chatId)
        {
            var command = ExtractCommand(message); // убирает "/"
            if (_adapters.TryGetValue(command, out var adapter))
            {
                var result = await adapter.ProcessMessage(message, chatId);
                if (!string.IsNullOrWhiteSpace(result.RedirectRouteKey))
                {
                    return await RouteAsync(result.RedirectRouteKey, chatId);
                }

                return result;
            }

            return new MessageResult { Text = "Неизвестная команда" };
        }

        /// <summary>
        /// Обрабатывает callback с параметрами в формате "route:param".
        /// </summary>
        /// <param name="callbackData">Данные callback (например, "role_select:admin").</param>
        /// <param name="chatId">ID чата Telegram.</param>
        /// <returns>Результат обработки callback.</returns>
        private async Task<MessageResult> RouteCallbackAsync(string callbackData, long chatId)
        {
            Console.WriteLine($"Пришел callback: {callbackData}");
            var parts = callbackData.Split(':', 2);
            var route = parts[0];
            var parameter = parts.Length > 1 ? parts[1] : "";

            if (_adapters.TryGetValue(route, out var adapter))
            {
                var result = await adapter.ProcessMessage(parameter, chatId);

                if (!string.IsNullOrEmpty(result.RedirectRouteKey))
                {
                    return await RouteAsync(result.RedirectRouteKey, chatId);
                }

                return result;
            }

            return new MessageResult { Text = "Неизвестный callback" };
        }

        /// <summary>
        /// Извлекает команду из сообщения (убирает "/" и приводит к нижнему регистру).
        /// </summary>
        /// <param name="message">Входящее сообщение.</param>
        /// <returns>Чистая команда без символа "/".</returns>
        private static string ExtractCommand(string message)
        {
            return message.Split(' ')[0]
                          .TrimStart('/')
                          .ToLowerInvariant();
        }
    }
}
