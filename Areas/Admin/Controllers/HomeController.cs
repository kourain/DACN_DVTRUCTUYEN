using DACN_DVTRUCTUYEN.Models;
using Microsoft.AspNetCore.Mvc;
using DACN_DVTRUCTUYEN.Utilities;
using System.Web;
namespace DACN_DVTRUCTUYEN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        public HomeController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [Route("/Admin")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("/Admin/Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [Route("/Admin/Login")]
        [HttpPost]
        public IActionResult Login(Login postdata)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new
                {
                    code = "0",
                    messenger = "Lỗi"
                });
            }
            if (string.IsNullOrEmpty(postdata.LoginName) || string.IsNullOrEmpty(postdata.PassWord))
            {
                return Ok(new
                {
                    code = "0",
                    messenger = "Thông tin không hợp lệ!"
                });
            }
            var temp = _dataContext.AdminUsers.Where(m => (m.Email == postdata.LoginName) && (m.PassWord == Functions.MD5Hash(postdata.PassWord))).FirstOrDefault();
            if (temp == null)
            {
                return Ok(new
                {
                    code = "0",
                    messenger = "Thông tin đăng nhập không chính xác!"
                });
            }
            var token = Functions.MD5Hash("admin" + postdata.LoginName + postdata.PassWord);
            var cookieoption = new CookieOptions() { Expires = DateTime.Now.AddHours(6) };
            if (postdata.savelogin == true)
            {
                cookieoption.Expires = DateTime.Now.AddDays(30);
            }
            Functions.saveLoginAdmin(token, postdata.LoginName, temp.AdminUserID);
            Response.Cookies.Append("username", HttpUtility.UrlEncode(temp.AdminName), cookieoption);
            Response.Cookies.Append("name", "admin", cookieoption);
            Response.Cookies.Append("id", postdata.LoginName, cookieoption);
            Response.Cookies.Append("token", token, cookieoption);
            return Ok(new
            {
                code = "1",
                messenger = "/Admin"
            });

        }
        [Route("/Admin/UserProfile")]
        [HttpGet]
        public IActionResult UserProfile()
        {
            return View();
        }

        [Route("/Admin/AdminChangePass")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminChangePass(ChangePass cp)
        {
            if (!ModelState.IsValid)
            {
                //return NotFound();
                return Ok(new
                {
                    code = "0",
                    messenger = "Thông tin không hợp lệ",
                    res2 = cp.oldPass + cp.newPass
                });
            }
            int adminid = Functions.IsLoginAdmin(Request.Cookies["token"], Request.Cookies["id"]);
            if (adminid == 0)
            {
                return Ok(new
                {
                    code = "0",
                    messenger = "Thông tin không hợp lệ",
                    res2 = "adminid = 0"
                });
            }
            var check = _dataContext.AdminUsers.Where(m => (m.AdminUserID == adminid) && (m.PassWord == Functions.MD5Hash(cp.oldPass))).FirstOrDefault();
            if (check == null)
            {
                return Ok(new
                {
                    code = "0",
                    messenger = "Thông tin không hợp lệ",
                    res2 = "pass/id"

                });
            }
            check.PassWord = Functions.MD5Hash(cp.newPass);
            _dataContext.Update(check);
            _dataContext.SaveChanges();
            //tạo token mới sau khi thay đổi mật khẩu
            var newtoken = Functions.MD5Hash("admin" + check.Email + check.PassWord);
            //cập nhật token trên trình duyệt khách
            Response.Cookies.Delete("token");
            Response.Cookies.Append("token",newtoken);
            //cập nhật token trên máy chủ
            Functions.tokenChangeAdmin(Request.Cookies["token"],newtoken);
            return Ok(new
            {
                code = "1",
                messenger = "Thành công"
            });
        }
    }
}
