using DACN_DVTRUCTUYEN.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace DACN_DVTRUCTUYEN.Areas.TelegramBot
{
    [Area("TelegramBot")]
    public class TelegramBot
    {
        string localurl = "https://localhost:44311";
        TelegramBotClient botClient = new TelegramBotClient("6729625846:AAE7A_DQvsy-_9nSeE_o1YyjVrrSm97qKYk");
        CancellationTokenSource cts = new();
        public TelegramBot()
        {
            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );
        }
        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
        };

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, $"{localurl}/telegrambot/api/messenger")
            {
                Content = JsonContent.Create(message)
            };
            try
            {
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode(); // Throws an exception if not successful

                var responseContent = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseContent))
                    Sendmess(chatId, responseContent);

                // Process the response content here
            }
            catch (HttpRequestException ex)
            {
                // Handle potential HTTP request errors here
                //Console.WriteLine("Error: " + ex.Message);
            }

            // Echo received message text
        }
        public void SendStaticMess(long? chatId, string messageText)
        {
            Sendmess(chatId, messageText);
        }
        public void Sendmess(long? chatId, string messageText)
        {
            if(chatId == null) return;
            Message mess = Task.Run(() => botClient.SendTextMessageAsync(
                chatId: chatId,
                text: messageText,
                cancellationToken: cts.Token)).Result;
        }

        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            return Task.CompletedTask;
        }

    }
    public class TelegramBotStatic
    {
        public static TelegramBot bot;
        public TelegramBotStatic()
        {
            bot = new TelegramBot();
        }
        public static void SendStaticMess(long? chatId, string messageText)
        {
            bot.Sendmess(chatId, messageText);
        }

    }


}
