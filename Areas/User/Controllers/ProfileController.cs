using DACN_DVTRUCTUYEN.Models;
using DACN_DVTRUCTUYEN.Utilities;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {

            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return BadRequest();
            }
            var us = _dataContext.Users.Where(m => m.UserId == userid).FirstOrDefault();
            return View(us);
        }
        [Route("/user/OrdersHistory")]
        public IActionResult OrdersHistory()
        {
            int.TryParse(Request.Cookies["id"], out int userid);
            if (Functions.IsLoginUser(Request.Cookies["token"], Request.Cookies["id"]) == 0)
            {
                return BadRequest();
            }
            var us = _dataContext.Orders.Where(m => m.UserID == userid).OrderByDescending(m=> m.Time).ToList();
            //test scroll table
            //us.AddRange(_dataContext.Orders.Where(m => m.UserID == userid).OrderByDescending(m => m.Time).AsEnumerable());
            //us.AddRange(_dataContext.Orders.Where(m => m.UserID == userid).OrderByDescending(m => m.Time).AsEnumerable());
            //us.AddRange(_dataContext.Orders.Where(m => m.UserID == userid).OrderByDescending(m => m.Time).AsEnumerable());
            //us.AddRange(_dataContext.Orders.Where(m => m.UserID == userid).OrderByDescending(m => m.Time).AsEnumerable());
            //us.AddRange(_dataContext.Orders.Where(m => m.UserID == userid).OrderByDescending(m => m.Time).AsEnumerable());
            //us.AddRange(_dataContext.Orders.Where(m => m.UserID == userid).OrderByDescending(m => m.Time).AsEnumerable());
            return View(us);
        }
        [Route("/user/totalpaid")]
        [HttpGet]
        public IActionResult totalpaid()
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
        public IActionResult password()
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
