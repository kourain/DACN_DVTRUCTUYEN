using Microsoft.AspNetCore.Mvc;
using DACN_DVTRUCTUYEN.Models;
using DACN_DVTRUCTUYEN.Utilities;
using Microsoft.AspNetCore.Hosting.Server;
using System;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Security.Cryptography;
using System.Web;
using NuGet.Common;

namespace DACN_DVTRUCTUYEN.Areas.User.Controllers
{
    [Area("User")]
    public class AuthenticatorController : Controller
    {
        private readonly DataContext _dataContext;
        public AuthenticatorController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return Login();
        }

        [Route("/User/Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [Route("/User/Login")]
        [HttpPost]
        public IActionResult Login(Login logindata)
        {
            if (string.IsNullOrEmpty(logindata.LoginName) || string.IsNullOrEmpty(logindata.PassWord))
            {
                return Ok(new
                {
                    code = 0,
                    messenger = "Dữ liệu không hợp lệ"
                });
            }
            string pw = Functions.MD5Hash(logindata.PassWord);
            logindata.LoginName = logindata.LoginName.ToLower();
            var check = _dataContext.Users.Where(m => (m.Email == logindata.LoginName) && (m.Password == pw)).FirstOrDefault();
            if (check == null)
            {
                return Ok(new
                {
                    code = 0,
                    messenger = "Tài khoản hoặc mật khẩu không chính xác"
                });
            }
            if (check.Ban == true)
            {
                return Ok(new
                {
                    code = 0,
                    messenger = "Tài khoản này đã bị cấm, vui lòng sử dụng 1 tài khoản khác"
                });
            }
            string token = Functions.MD5Hash("user" + logindata.LoginName + logindata.PassWord);
            Functions.saveLoginUser(token, check.UserId.ToString(),logindata.LoginName, logindata.savelogin);
            Response.Cookies.Append("username", check.Name);
            Response.Cookies.Append("id", check.UserId.ToString());
            Response.Cookies.Append("mail", logindata.LoginName);
            Response.Cookies.Append("token", token);
            return Ok(new
            {
                code = 1,
                messenger = "/User"
            });
        }
        [Route("/User/Register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [Route("/User/Register")]
        [HttpPost]
        public IActionResult Register(Models.User user)
        {
            if (user == null)
            {
                return Ok(new
                {
                    code = 0,
                    messenger = "Dữ liệu không hợp lệ"
                });
            }
            var check = _dataContext.Users.Where(m => m.Email == user.Email).FirstOrDefault();
            if (check != null)
            {
                return Ok(new
                {
                    code = 0,
                    messenger = "Email này đã được sử dụng"
                });
            }
            user.Email = user.Email.ToLower();
            user.Password = Functions.MD5Hash(user.Password);
            _dataContext.Add(user);
            _dataContext.SaveChanges();
            return Ok(new
            {
                code = 1,
                messenger = "/User"
            });
        }
        [Route("/User/ForgotPass")]
        [HttpGet]
        public IActionResult ForgotPass()
        {
            return View();
        }
        [Route("/User/ForgotPass")]
        [HttpPost]
        public ActionResult ForgotPass(Models.User user)
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                return Ok(new
                {
                    code = 0,
                    messenger = "Dữ liệu không hợp lệ"
                });
            }
            var check = _dataContext.Users.Where(m => (m.Email == user.Email) && (m.Email == user.Email)).FirstOrDefault();
            if (check == null)
            {
                return Ok(new
                {
                    code = 0,
                    messenger = "Thông tin không khớp"
                });
            }
            else
            {
                check.Password = null;
                _dataContext.Update(check);
                _dataContext.SaveChanges();
            }
            return Ok(new
            {
                code = 1,
                messenger = "Vui lòng kiểm tra mật khẩu mới có trong hòm thư điện tử"
            });
        }
        [Route("/User/ChangePass")]
        [HttpGet]
        public IActionResult ChangePass()
        {
            return View();
        }
        [Route("/User/ChangePass")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePass(ChangePass cp)
        {
            //var check = _dataContext.Users.Where(m => (m.UserId == Functions._UserID) && (m.Password == Functions.MD5Password(cp.oldPass))).FirstOrDefault();
            //if (check == null)
            //{
            //    Functions._Message = "Mật khẩu cũ không chính xác";
            //    return View();
            //}
            //else
            //{
            //    check.Password = Functions.MD5Password(cp.newPass);
            //    _dataContext.Update(check);
            //    _dataContext.SaveChanges();
            //}
            //Functions._Message = "Thành Công!!!";
            return View();
        }
    }
}
