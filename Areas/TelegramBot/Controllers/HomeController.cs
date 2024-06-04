using DACN_DVTRUCTUYEN.Models;
using DACN_DVTRUCTUYEN.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
namespace DACN_DVTRUCTUYEN.Areas.TelegramBot.Controllers
{
    [Area("TelegramBot")]
    public class HomeController : Controller
    {
        private static DataContext _dataContext;
        public HomeController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public static string messenger(Message data)
        {
            var text = data.Text;
            if (text == null) { return ""; }
            var textarr = text.Split(" ");
            switch (textarr[0])
            {
                case "/start":
                    if (textarr.Length > 1)
                    {
                        int? usid = Functions.GetUIDFromToken(textarr[1]);
                        if (usid != 0 && usid != null)
                        {
                            var us = _dataContext.Users.Where(m => m.UserId == usid).FirstOrDefault();
                            us.TelegramChatID = data.Chat.Id;
                            us.TelegramName = data.From.FirstName + " " + data.From.LastName;
                            us.TelegramUserName = data.From.Username;
                            _dataContext.Update(us);
                            _dataContext.SaveChanges();
                            return $"Bạn đã liên kết thành công với tài khoản: {us.Name} bằng tài khoản Telegram: @{us.TelegramUserName}";
                        }
                    }
                    else
                    {
                        return $"Vui lòng sử dụng nút liên kết Telegram từ trang thông tin cá nhân của trang web!";
                    }
                    break;
                default:
                    break;
            }
            return "";
            // Process the rawRequestBody
        }
        [Route("/telegrambot/api/messenger")]
        [HttpPost]

        public async Task<IActionResult> messenger()
        {
            using (var reader = new StreamReader(Request.Body, encoding: Encoding.UTF8))
            {
                var rawRequestBody = reader.ReadToEndAsync().Result;
                var data = JsonConvert.DeserializeObject<JObject>(rawRequestBody);
                var text = data["text"].ToString();
                if (text == null) { return BadRequest(); }
                var textarr = text.Split(" ");
                switch (textarr[0])
                {
                    case "/start":
                        if (textarr.Length > 1)
                        {
                            int? usid = Functions.GetUIDFromToken(textarr[1]);
                            if (usid != 0 && usid != null)
                            {
                                var us = _dataContext.Users.Where(m => m.UserId == usid).FirstOrDefault();
                                us.TelegramChatID = (long)data["chat"]["id"];
                                us.TelegramName = data["from"]["firstName"].ToString() + " " + data["from"]["lastName"].ToString();
                                us.TelegramUserName = data["from"]["username"].ToString();
                                _dataContext.Update(us);
                                _dataContext.SaveChanges();
                                return Ok($"Bạn đã liên kết thành công với tài khoản: {us.Name} bằng tài khoản Telegram: @{data["from"]["username"]}");
                            }
                        }
                        else
                        {
                            return Ok($"Vui lòng sử dụng nút liên kết Telegram từ trang thông tin cá nhân của trang web!");
                        }
                        break;
                    default:
                        break;
                }
                Console.WriteLine(rawRequestBody);
                return Ok("");
                // Process the rawRequestBody
            }
        }
        protected static TelegramBotClient botClient = new TelegramBotClient("6729625846:AAE7A_DQvsy-_9nSeE_o1YyjVrrSm97qKYk");
        protected static CancellationTokenSource cts = new();
        public static void Start()
        {
            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );
        }
        // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        static ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
        };

        static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;

            try
            {
                SendMess(chatId, messenger(message));

                // Process the response content here
            }
            catch (HttpRequestException ex)
            {
                // Handle potential HTTP request errors here
                //Console.WriteLine("Error: " + ex.Message);
            }

            // Echo received message text
        }
        public static void SendMess(long? chatId, string messageText)
        {
            if (chatId == null) return;
            Message mess = Task.Run(() => botClient.SendTextMessageAsync(
                chatId: chatId,
                text: messageText,
                cancellationToken: cts.Token)).Result;
        }

        static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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

}
