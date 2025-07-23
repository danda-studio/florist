using FloristAI.Adapter;
using FloristAI.Adapter.Models;
using Telegram.Bot.Types;


namespace FloristAI.Router
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
        public async Task<List<MessageResult>> Route(string message, long chatId)
        {
            if (string.IsNullOrWhiteSpace(message))
                new List<MessageResult> { new MessageResult { Text = "Неизвестная команда" } };

            if (message.Contains(":"))
                return await RouteCallback(message, chatId);
            else if (message.StartsWith("/"))
                return await RouteCommand(message, chatId);

            return await RouteTextInput(message, chatId);
        }

        private async Task<List<MessageResult>> RouteTextInput(string message, long chatId)
        {
            if (_adapters.TryGetValue("step_input", out var stepInputAdapter))
            {
                return await stepInputAdapter.ProcessMessage(message, chatId);
            }
            return new List<MessageResult>
            {
                new MessageResult { Text = "Неизвестный шаг ввода текста" }
            };
        }

        /// <summary>
        /// Обрабатывает команду (текстовое сообщение без параметров).
        /// </summary>
        /// <param name="message">Команда (например, "/start").</param>
        /// <param name="chatId">ID чата Telegram.</param>
        /// <returns>Результат обработки команды.</returns>
        private async Task<List<MessageResult>> RouteCommand(string message, long chatId)
        {
            var command = ExtractCommand(message); // убирает "/"
            if (_adapters.TryGetValue(command, out var adapter))
            {
                var result = await adapter.ProcessMessage(message, chatId);

                // Так как result — список, проверяем RedirectRouteKey у первого сообщения
                var firstMessage = result.FirstOrDefault();

                if (firstMessage != null && !string.IsNullOrWhiteSpace(firstMessage.RedirectRouteKey))
                {
                    return await Route(firstMessage.RedirectRouteKey, chatId);
                }

                return result;
            }
            return new List<MessageResult> { new MessageResult { Text = $"Неизвестная команда: {command}" } };
        }

        /// <summary>
        /// Обрабатывает callback с параметрами в формате "route:param".
        /// </summary>
        /// <param name="callbackData">Данные callback (например, "role_select:admin").</param>
        /// <param name="chatId">ID чата Telegram.</param>
        /// <returns>Результат обработки callback.</returns>
        private async Task<List<MessageResult>> RouteCallback(string callbackData, long chatId)
        {
            Console.WriteLine($"Пришел callback: {callbackData}");

            if (callbackData.StartsWith("step_message:"))
            {
                var step = callbackData.Substring("step_message:".Length);

                if (_adapters.TryGetValue("step_message", out var stepAdapter))
                {
                    return await stepAdapter.ProcessMessage(step, chatId);
                }
            }
            var parts = callbackData.Split(':', 2);
            var route = parts[0];
            var parameter = parts.Length > 1 ? parts[1] : "";

            if (_adapters.TryGetValue(route, out var adapter))
            {
                var result = await adapter.ProcessMessage(parameter, chatId);

                // Так как result — список, проверяем RedirectRouteKey у первого сообщения
                var firstMessage = result.FirstOrDefault();

                if (firstMessage != null && !string.IsNullOrWhiteSpace(firstMessage.RedirectRouteKey))
                {
                    return await Route(firstMessage.RedirectRouteKey, chatId);
                }

                return result;
            }
            return new List<MessageResult> { new MessageResult { Text = $"Неизвестный callback: {callbackData}" } };
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
