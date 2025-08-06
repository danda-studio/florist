using FloristAI.Adapter.Models;
using FloristAI.Router;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace FloristAI.Infrastructure
{
    public class BotWorker : BackgroundService
    {
        private static readonly Dictionary<long, List<int>> _lastBotMessages = new();
        private static readonly Dictionary<long, int> _pinnedMessages = new();

        private readonly ITelegramBotClient _botClient;
        private readonly AdapterRouter _router;

        public BotWorker(ITelegramBotClient botClient, AdapterRouter router)
        {
            _botClient = botClient;
            _router = router;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _botClient.StartReceiving(
                async (bot, update, token) =>
                {
                    try
                    {
                        if (update.Message?.Text != null)
                        {
                            var chatId = update.Message.Chat.Id;
                            var input = update.Message.Text;

                            var results = await _router.Route(input, chatId);
                            await _botClient.DeleteMessage(chatId, update.Message.MessageId);

                            await ProcessResults(chatId, results, token);
                        }
                        else if (update.CallbackQuery != null)
                        {
                            var chatId = update.CallbackQuery.Message?.Chat.Id ?? update.CallbackQuery.From.Id;
                            var messageId = update.CallbackQuery.Message?.MessageId ?? 0;
                            var command = update.CallbackQuery.Data ?? "";

                            var results = await _router.Route(command, chatId);

                            if (messageId != 0)
                            {
                                try
                                {
                                    await _botClient.DeleteMessage(chatId, messageId);
                                }
                                catch { }
                            }

                            await ProcessResults(chatId, results, token);
                            await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при обработке обновления: {ex.Message}");
                    }
                },
                HandleErrorAsync,
                new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() },
                stoppingToken
            );

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        /// <summary>
        /// Общая обработка результатов: удаление старых сообщений, снятие закрепа, отправка новых.
        /// </summary>
        private async Task ProcessResults(long chatId, List<MessageResult> results, CancellationToken token)
        {
            if (results.Any(r => r.RemovePinnedMessage) && _pinnedMessages.TryGetValue(chatId, out var pinnedId))
            {
                try
                {
                    await _botClient.UnpinChatMessage(chatId, pinnedId, cancellationToken: token);
                    await _botClient.DeleteMessage(chatId, pinnedId, cancellationToken: token);
                    _pinnedMessages.Remove(chatId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при удалении закрепа: {ex.Message}");
                }
            }

            if (_lastBotMessages.TryGetValue(chatId, out var oldMessages))
            {
                foreach (var oldId in oldMessages)
                {
                    try { await _botClient.DeleteMessage(chatId, oldId, cancellationToken: token); }
                    catch { }
                }
                oldMessages.Clear();
            }

            foreach (var result in results)
            {
                Message sentMessage;

                if (result.Photo?.ImageBytes != null)
                {
                    using var stream = new MemoryStream(result.Photo.ImageBytes);
                    sentMessage = await _botClient.SendPhoto(
                        chatId: chatId,
                        photo: InputFile.FromStream(stream),
                        caption: result.Text,
                        replyMarkup: result.ReplyMarkup,
                        cancellationToken: token
                    );
                }
                else
                {
                    sentMessage = await _botClient.SendMessage(
                        chatId: chatId,
                        text: result.Text,
                        replyMarkup: result.ReplyMarkup,
                        cancellationToken: token
                    );
                }

                if (result.PinnedMessage)
                {
                    await _botClient.PinChatMessage(chatId, sentMessage.MessageId, cancellationToken: token);
                    _pinnedMessages[chatId] = sentMessage.MessageId;
                }
                else
                {
                    if (!_lastBotMessages.ContainsKey(chatId))
                        _lastBotMessages[chatId] = new List<int>();

                    _lastBotMessages[chatId].Add(sentMessage.MessageId);
                }
            }
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken token)
        {
            Console.WriteLine($"Ошибка Telegram: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}
