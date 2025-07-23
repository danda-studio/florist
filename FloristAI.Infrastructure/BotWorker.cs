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

        private static Dictionary<long, List<int>> _lastBotMessages = new();
        private static Dictionary<long, int> _pinnedMessages = new(); 

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
                            var results = await _router.Route(message.Text, message.Chat.Id); 

                            await _botClient.DeleteMessage(message.Chat.Id, message.MessageId, cancellationToken: token);

                            if (_lastBotMessages.TryGetValue(message.Chat.Id, out var lastMessageId))
                            {
                                foreach(var messageId in lastMessageId)
                                {
                                    if (_pinnedMessages.TryGetValue(message.Chat.Id, out var pinnedId) && pinnedId == messageId)
                                        continue;

                                    try
                                    {
                                        await _botClient.DeleteMessage(message.Chat.Id, messageId, cancellationToken: token);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Ошибка удаления: {ex.Message}");
                                    }
                                }
                                
                            }

                            foreach (var result in results)
                            {
                                if (result.Photo?.ImageBytes != null)
                                {
                                    using var stream = new MemoryStream(result.Photo.ImageBytes);
                                    var sentPhoto = await _botClient.SendPhoto(
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

                                    if (result.PinnedMessage)
                                    {
                                        await _botClient.PinChatMessage(message.Chat.Id, sentMessage.MessageId);
                                        _pinnedMessages[message.Chat.Id] = sentMessage.MessageId;
                                    }

                                    if (results.Any(r => r.RemovePinnedMessage) && _pinnedMessages.TryGetValue(message.Chat.Id, out var pinnedId))
                                    {
                                        try
                                        {
                                            await _botClient.UnpinAllChatMessages(message.Chat.Id);
                                            await _botClient.DeleteMessage(message.Chat.Id, pinnedId);
                                            _pinnedMessages.Remove(message.Chat.Id);
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Ошибка при удалении закрепа: {ex.Message}");
                                        }
                                    }

                                    if (!_lastBotMessages.ContainsKey(message.Chat.Id))
                                        _lastBotMessages[message.Chat.Id] = new List<int>();

                                    if(!result.PinnedMessage)
                                    _lastBotMessages[message.Chat.Id].Add(sentMessage.MessageId);
                                }
                            }
                        }
                        else if (update.CallbackQuery != null)
                        {
                            var callback = update.CallbackQuery;
                            var chatId = callback.Message?.Chat.Id ?? callback.From.Id;
                            var messageId = callback.Message?.MessageId ?? 0;

                            string command = callback.Data ?? "";

                            var results = await _router.Route(command, chatId); 

                            if (messageId != 0)
                            {
                                try
                                {
                                    await _botClient.DeleteMessage(chatId, messageId, cancellationToken: token);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Ошибка удаления сообщения с кнопкой: {ex.Message}");
                                }
                            }

                            if (_lastBotMessages.TryGetValue(chatId, out var lastMessageId))
                            {

                                foreach(var messageIds in lastMessageId)
                                {
                                    if (_pinnedMessages.TryGetValue(chatId, out var pinnedId) && pinnedId == messageIds)
                                        continue;

                                    try
                                    {
                                        await _botClient.DeleteMessage(chatId, messageIds, cancellationToken: token);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Ошибка удаления: {ex.Message}");
                                    }
                                } 
                            }

                            foreach (var result in results)
                            {
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
                                    var sentMessage = await _botClient.SendMessage(
                                        chatId: chatId,
                                        text: result.Text,
                                        replyMarkup: result.ReplyMarkup,
                                        cancellationToken: token
                                    );

                                    if (result.PinnedMessage)
                                    {
                                        await _botClient.PinChatMessage(chatId, sentMessage.MessageId);
                                        _pinnedMessages[chatId] = sentMessage.MessageId;
                                    }

                                    if (results.Any(r => r.RemovePinnedMessage) && _pinnedMessages.TryGetValue(chatId, out var pinnedId))
                                    {
                                        try
                                        {
                                            await _botClient.UnpinAllChatMessages(chatId);
                                            await _botClient.DeleteMessage(chatId, pinnedId);
                                            _pinnedMessages.Remove(chatId);
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Ошибка при удалении закрепа: {ex.Message}");
                                        }
                                    }

                                    if (!_lastBotMessages.ContainsKey(chatId))
                                        _lastBotMessages[chatId] = new List<int>();

                                    if (!result.PinnedMessage)
                                        _lastBotMessages[chatId].Add(sentMessage.MessageId);
                                }
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
                    AllowedUpdates = Array.Empty<UpdateType>() 
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
