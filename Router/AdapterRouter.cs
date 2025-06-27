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
        /// Словарь адаптеров
        /// </summary>
        private readonly Dictionary<string, IMessageAdapter> _adapters;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AdapterRouter"/>.
        /// </summary>
        /// <param name="adapters">Коллекция доступных адаптеров, реализующих интерфейс <see cref="IMessageAdapter"/>.</param>
        public AdapterRouter(IEnumerable<IMessageAdapter> adapters)
        {
            _adapters = adapters.ToDictionary(a => a.RouteKey);
            Console.WriteLine($"Зарегистрированные ключи: {string.Join(", ", _adapters.Keys)}"); 
        }


        public async Task<MessageResult> RouteAsync(string message, long chatId)
        {
            if (string.IsNullOrWhiteSpace(message))
                return new MessageResult { Text = "Неизвестная команда" };

            if (message.Contains(":"))
                return await RouteCallbackAsync(message, chatId);
            else
                return await RouteCommandAsync(message, chatId);
        }

        private async Task<MessageResult> RouteCommandAsync(string message, long chatId)
        {
            var command = ExtractCommand(message); // уберёт "/" 
            if (_adapters.TryGetValue(command, out var adapter))
                return await adapter.ProcessMessage(message, chatId);

            return new MessageResult { Text = "Неизвестная команда" };
        }

        private async Task<MessageResult> RouteCallbackAsync(string callbackData, long chatId)
        {
            Console.WriteLine($"Пришел callback: {callbackData}");
            var parts = callbackData.Split(':', 2);
            var route = parts[0];
            var parameter = parts.Length > 1 ? parts[1] : "";

            if (_adapters.TryGetValue(route, out var adapter))
                return await adapter.ProcessMessage(parameter, chatId);

            return new MessageResult { Text = "Неизвестный callback" };
        }

        private static string ExtractCommand(string message)
        {
            return message.Split(' ')[0]
                          .TrimStart('/')
                          .ToLowerInvariant();
        }
    }
}
