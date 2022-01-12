using Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Task11.Properties;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Task11 {
    public class TelegramReplyingService {
        public async Task StartListening() {
            using var cts = new CancellationTokenSource();

            var botClient = new TelegramBotClient(Resources.TelegramBotKey);
            var receiverOptions = new ReceiverOptions {
                AllowedUpdates = { } // receive all update types
            };

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token);

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Type != UpdateType.Message) {
                return;
            }
            // Only process text messages
            if (update.Message!.Type != MessageType.Text) {
                return;
            }

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;
            var service = new GetCurrencyRateService();
            var answerText = await service.GetData(messageText);

            //return asked currency rates
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: answerText,
                cancellationToken: cancellationToken);
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken) {
            var ErrorMessage = exception switch {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
