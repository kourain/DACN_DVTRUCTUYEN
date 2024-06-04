using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DACN_DVTRUCTUYEN.Areas.User.Models;
using DACN_DVTRUCTUYEN.Models;
using DACN_DVTRUCTUYEN.Utilities;

namespace DACN_DVTRUCTUYEN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly DataContext _Context;
        public UsersController(DataContext context)
        {
            _Context = context;
        }
        public async Task<IActionResult> Index()
        {

            var mnlist = _Context.Users.OrderBy(m => m.UserId).ToList();
            return View(mnlist);
        }
        [HttpGet]
        public async Task<IActionResult> Ban(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var mn = _Context.Users.FirstOrDefault(m => m.UserId == id);
            if (mn == null)
            {
                return NotFound();
            }
            return View(mn);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ban(User.Models.User us)
        {
            var banuser = _Context.Users.FirstOrDefault(m=> m.UserId == us.UserId);
            if (banuser == null)
            {
                return NotFound();
            }
            if(banuser.Ban == true)
            {
                TelegramBot.Controllers.HomeController.SendMess(banuser.TelegramChatID, $"Chào {banuser.Name}:\n" +
                    $"\tTài khoản DB Shop {banuser.Email} của bạn vừa được gỡ trạng thái cấm\n" +
                    $"\tHãy tới trang web mua hàng ngay nào!");
            }
            else
            {
                TelegramBot.Controllers.HomeController.SendMess(banuser.TelegramChatID, $"Chào {banuser.Name}:\n" +
                    $"\tTài khoản DB Shop {banuser.Email} của bạn vừa bị cấm\n" +
                    $"\tLý do: {us.BanReason}");
            }
            banuser.Ban = !banuser.Ban;
            banuser.BanReason = us.BanReason;
            _Context.Users.Update(banuser);
            _Context.SaveChanges();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> ResetPassWord(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var mn = _Context.Users.Find(id);
            if (mn == null)
            {
                return NotFound();
            }
            return View(mn);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassWord(User.Models.User mn)
        {
            var resetUser = _Context.Users.Find(mn.UserId);
            if (resetUser == null)
            {
                return NotFound();
            }
            resetUser.Password = Functions.MD5Hash("12345678");
            _Context.Users.Update(resetUser);
            _Context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
