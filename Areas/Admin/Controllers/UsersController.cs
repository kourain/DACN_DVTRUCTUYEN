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
        public async Task<IActionResult> Ban(int? id)
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
        public async Task<IActionResult> Ban(int id)
        {
            var deleUser = _Context.Users.Find(id);
            if (deleUser == null)
            {
                return NotFound();
            }
            deleUser.Ban = !deleUser.Ban;
            _Context.Users.Update(deleUser);
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
