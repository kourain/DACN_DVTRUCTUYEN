using DACN_DVTRUCTUYEN.Models;
using DACN_DVTRUCTUYEN.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Telegram.Bot.Types;
namespace DACN_DVTRUCTUYEN.Areas.TelegramBot.Controllers
{
    [Area("TelegramBot")]
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        public HomeController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public string messenger(JObject data)
        {
            var text = data["text"].ToString();
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
                            us.TelegramChatID = (long)data["chat"]["id"];
                            us.TelegramName = data["from"]["firstName"].ToString() + " " + data["from"]["lastName"].ToString();
                            us.TelegramUserName = data["from"]["username"].ToString();
                            _dataContext.Update(us);
                            _dataContext.SaveChanges();
                            return $"Bạn đã liên kết thành công với tài khoản: {us.Name} bằng tài khoản Telegram: @{data["from"]["username"]}";
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
    }

}
