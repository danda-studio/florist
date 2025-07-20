using FloristAI.Router;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FloristAI.Infrastructure
{
    /// <summary>
    /// Фоновый сервис для запуска и обработки Telegram-бота.
    /// </summary>
    public class BotWorker : BackgroundService
    {

        private static Dictionary<long, int> _lastBotMessages = new();
        /// <summary>
        /// Клиент Telegram-бота.
        /// </summary>
        private readonly ITelegramBotClient _botClient;

        /// <summary>
        /// Маршрутизатор сообщений.
        /// </summary>
        private readonly AdapterRouter _router;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="BotWorker"/>.
        /// </summary>
        /// <param name="botClient">Клиент Telegram-бота.</param>
        /// <param name="router">Маршрутизатор сообщений.</param>
        public BotWorker(ITelegramBotClient botClient, AdapterRouter router)
        {
            _botClient = botClient;
            _router = router;
        }

        /// <summary>
        /// Основной метод сервиса, который запускает получение обновлений от Telegram.
        /// </summary>
        /// <param name="stoppingToken">Токен отмены для остановки сервиса.</param>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _botClient.StartReceiving(
                async (bot, update, token) =>
                {
                    try
                    {
                        if (update.Message?.Text != null)
                        {
                            var message = update.Message;
                            var result = await _router.Route(message.Text, message.Chat.Id);

                            // Удаляем входящее сообщение
                            await _botClient.DeleteMessage(message.Chat.Id, message.MessageId, cancellationToken: token);

                            // Если есть фото, отправляем его
                            if (result.Photo?.ImageBytes != null)
                            {
                                using var stream = new MemoryStream(result.Photo.ImageBytes);
                                await _botClient.SendPhoto(
                                    chatId: message.Chat.Id,
                                    photo: InputFile.FromStream(stream),
                                    caption: result.Text,
                                    replyMarkup: result.ReplyMarkup,
                                    cancellationToken: token
                                );
                            }
                            else
                            {
                                var sentMessage = await _botClient.SendMessage(
                                    chatId: message.Chat.Id,
                                    text: result.Text,
                                    replyMarkup: result.ReplyMarkup,
                                    cancellationToken: token
                                );

                                _lastBotMessages[message.Chat.Id] = sentMessage.MessageId;
                            }
                        }
                        else if (update.CallbackQuery != null)
                        {
                            var callback = update.CallbackQuery;
                            var chatId = callback.Message?.Chat.Id ?? callback.From.Id;
                            var messageId = callback.Message?.MessageId ?? 0;

                            string command = callback.Data ?? "";

                            var result = await _router.Route(command, chatId);

                            if(messageId != 0)
                            {
                                await _botClient.DeleteMessage(chatId, messageId, cancellationToken: token);
                            }

                            // Если есть фото, отправляем его
                            if (result.Photo?.ImageBytes != null)
                            {
                                using var stream = new MemoryStream(result.Photo.ImageBytes);
                                await _botClient.SendPhoto(
                                    chatId: chatId,
                                    photo: InputFile.FromStream(stream),
                                    caption: result.Text,
                                    replyMarkup: result.ReplyMarkup,
                                    cancellationToken: token
                                );
                            }
                            else
                            {
                                await _botClient.SendMessage(
                                    chatId: chatId,
                                    text: result.Text,
                                    replyMarkup: result.ReplyMarkup,
                                    cancellationToken: token
                                );
                            }

                            await _botClient.AnswerCallbackQuery(callback.Id, cancellationToken: token);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при обработке сообщения: {ex.Message}");
                    }
                },
                HandleErrorAsync,
                new ReceiverOptions
                {
                    AllowedUpdates = Array.Empty<UpdateType>() // получаем всё
                },
                cancellationToken: stoppingToken
            );

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        /// <summary>
        /// Обработка ошибок, возникающих при работе Telegram-бота.
        /// </summary>
        /// <param name="botClient">Клиент Telegram-бота.</param>
        /// <param name="exception">Возникшее исключение.</param>
        /// <param name="token">Токен отмены.</param>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken token)
        {
            Console.WriteLine($"Ошибка Telegram: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}
