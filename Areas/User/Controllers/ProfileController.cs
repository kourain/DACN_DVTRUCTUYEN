using DACN_DVTRUCTUYEN.Models;
using DACN_DVTRUCTUYEN.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace DACN_DVTRUCTUYEN.Areas.User.Controllers
{
    [Area("User")]
    public class ProfileController : Controller
    {
        private readonly DataContext _dataContext;
        public ProfileController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [Route("/user/profile")]
        public async Task<IActionResult> Index()
        {

            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return Redirect("/user");
            }
            var us = _dataContext.Users.Where(m => m.UserId == userid).FirstOrDefault();
            return View(us);
        }
        [Route("/user/OrdersHistory")]
        public async Task<IActionResult> OrdersHistory(int paystatus = 0, string sort = "asc", int date = 7)
        {
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return Redirect("/user");
            }
            var result = _dataContext.Orders.Where(m => m.UserID == userid && m.Time >= DateTime.Now.AddDays(-date));

            switch (paystatus)
            {
                case -2:
                    result = result.Where(m => m.PayStatus == -2);
                    break;
                case 2:
                    result = result.Where(m => m.PayStatus == 2);
                    break;
                default:
                    break;
            }
            if (sort == "asc")
                result = result.OrderByDescending(m => m.Time);
            else
            {
                result = result.OrderBy(m => m.Time);
            }
            //test scroll table
            //us.AddRange(_dataContext.Orders.Where(m => m.UserID == userid).OrderByDescending(m => m.Time).AsEnumerable());
            //us.AddRange(_dataContext.Orders.Where(m => m.UserID == userid).OrderByDescending(m => m.Time).AsEnumerable());
            //us.AddRange(_dataContext.Orders.Where(m => m.UserID == userid).OrderByDescending(m => m.Time).AsEnumerable());
            //us.AddRange(_dataContext.Orders.Where(m => m.UserID == userid).OrderByDescending(m => m.Time).AsEnumerable());
            //us.AddRange(_dataContext.Orders.Where(m => m.UserID == userid).OrderByDescending(m => m.Time).AsEnumerable());
            //us.AddRange(_dataContext.Orders.Where(m => m.UserID == userid).OrderByDescending(m => m.Time).AsEnumerable());
            return View(await result.ToListAsync());
        }
        [Route("/user/totalpaid")]
        [HttpGet]
        public async Task<IActionResult> totalpaid()
        {
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return BadRequest();
            }
            var us = _dataContext.Users.Where(m => m.UserId == userid).FirstOrDefault();
            return Ok(new
            {
                code = 1,
                messenger = us.TotalPaid
            });
        }
        [Route("/user/password")]
        [HttpGet]
        public async Task<IActionResult> password()
        {
            return View();
        }
        // MOVE TO Authenticator Controller
        //[Route("/user/password")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult password(ChangePass cp)
        //{
        //    return View();
        //}
    }
}
